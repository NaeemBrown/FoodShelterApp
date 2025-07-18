/*document.addEventListener('DOMContentLoaded', function() {
    const foodStockForm = document.getElementById('foodStockForm');
    if (foodStockForm) {
        foodStockForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const formData = new FormData(this);
            
            fetch('/FoodStock/Create', {
                method: 'POST',
                body: formData
            })
            .then(response => {
                if (response.redirected) {
                    window.location.href = response.url;
                    return;
                }
                return response.json();
            })
            .catch(error => {
                console.error('Error:', error);
            });
        });
    }
});*/

function editFoodStock(id) {
    const row = document.querySelector(`tr[data-item-id="${id}"]`);
    if (!row) return;
    
    // Store original values
    const originalValues = {};
    row.querySelectorAll('[data-field]').forEach(cell => {
        originalValues[cell.dataset.field] = cell.textContent;
        
        // Create input field
        const input = document.createElement('input');
        input.type = cell.dataset.field === 'ExpirationDate' ? 'date' : 
                     cell.dataset.field === 'Quantity' || cell.dataset.field === 'MinimumStock' ? 'number' : 'text';
        
        // Set attributes for number fields
        if (cell.dataset.field === 'Quantity') {
            input.step = '0.01';
            input.min = '0';
            // Parse the decimal number correctly
            const value = cell.textContent.trim();
            input.value = value;
            input.setAttribute('data-original', value);
            
            // Add specific validation for quantity
            input.addEventListener('input', function(e) {
                const value = parseFloat(e.target.value);
                if (isNaN(value) || value < 0) {
                    e.target.setCustomValidity('Quantity must be a positive number');
                } else {
                    e.target.setCustomValidity('');
                }
            });
        } else if (cell.dataset.field === 'MinimumStock') {
            input.type = 'number';
            input.min = '0';
            input.step = '1';
            input.value = cell.textContent.trim();
        } else {
            input.value = cell.dataset.field === 'ExpirationDate' && cell.textContent === 'N/A' ? '' : cell.textContent;
        }
        
        input.className = 'inline-edit-input';
        input.required = true;
        input.addEventListener('invalid', function(e) {
            if (e.target.validity.valueMissing) {
                e.target.setCustomValidity(`${cell.dataset.field} cannot be empty`);
            }
        });
        
        // Clear validation message when user starts typing
        input.addEventListener('input', function(e) {
            if (e.target.validity.valueMissing) {
                e.target.setCustomValidity('');
            }
        });
        
        // Clear cell and add input
        cell.textContent = '';
        cell.appendChild(input);
    });

    // Replace edit button with save/cancel buttons
    const actionCell = row.querySelector('.action-buttons');
    actionCell.innerHTML = `
        <button class="dashboard-btn dashboard-btn-success" onclick="saveEdit(${id})">
            <i class="fas fa-save"></i> Save
        </button>
        <button class="dashboard-btn dashboard-btn-secondary" onclick="cancelEdit(${id}, ${JSON.stringify(originalValues)})">
            <i class="fas fa-times"></i> Cancel
        </button>
    `;
}

function saveEdit(id) {
    const row = document.querySelector(`tr[data-item-id="${id}"]`);
    if (!row) return;

    const inputs = row.querySelectorAll('.inline-edit-input');
    let isValid = true;

    inputs.forEach(input => {
        if (!input.checkValidity()) {
            input.reportValidity();
            isValid = false;
        }
    });

    if (!isValid) return;

    const updatedValues = {};
    row.querySelectorAll('[data-field]').forEach(cell => {
        const input = cell.querySelector('input');
        if (input) {
            updatedValues[cell.dataset.field] = input.value;
        }
    });

    fetch(`/FoodStock/Update/${id}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(updatedValues)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            location.reload();
        } else {
            alert(data.message || 'Error updating food stock item');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error updating food stock item');
    });
}

function cancelEdit(id, originalValues) {
    const row = document.querySelector(`tr[data-item-id="${id}"]`);
    if (!row) return;

    // Restore original values
    Object.entries(originalValues).forEach(([field, value]) => {
        const cell = row.querySelector(`[data-field="${field}"]`);
        cell.textContent = value;
    });

    // Restore original buttons
    const actionCell = row.querySelector('.action-buttons');
    actionCell.innerHTML = `
        <button class="dashboard-btn dashboard-btn-edit" onclick="editFoodStock(${id})">
            <i class="fas fa-edit"></i> Edit
        </button>
        <form asp-controller="FoodStock" asp-action="Delete" asp-route-id="${id}" method="post" class="inline-form">
            <button type="submit" class="dashboard-btn dashboard-btn-delete">
                <i class="fas fa-trash"></i> Delete
            </button>
        </form>
    `;
}

document.addEventListener('DOMContentLoaded', function () {
    // Setup modal buttons
    const buttons = {
        'addFoodStockBtn': 'foodStockModal',
        'addShelterLocationBtn': 'shelterLocationModal',
        'addNoteBtn': 'noteModal',
        'addVolunteerBtn': 'volunteerModal',
        'addBudgetEntryBtn': 'budgetModal',
        'addDonationBtn': 'donationModal'
    };

    // Setup each modal and its button
    Object.entries(buttons).forEach(([buttonId, modalId]) => {
        const button = document.getElementById(buttonId);
        const modal = document.getElementById(modalId);
        
        if (button && modal) {
            const closeBtn = modal.querySelector('.close');
            
            // Open modal
            button.onclick = () => modal.style.display = 'block';
            
            // Close modal
            closeBtn.onclick = () => modal.style.display = 'none';
            
            // Close on outside click
            window.onclick = (e) => {
                if (e.target == modal) modal.style.display = 'none';
            }
        }
    });

    // Setup form submissions
    const forms = [
        //'foodStockForm',
        'shelterLocationForm',
        'noteForm',
        'volunteerForm',
        'budgetForm',
        'donationForm'
    ];

    forms.forEach(formId => {
        const form = document.getElementById(formId);
        if (form) {
            form.addEventListener('submit', handleFormSubmit);
        }
    });


    // Handle form submission for both add and edit
    /*document.getElementById('foodStockForm').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const formData = new FormData(this);
        const url = this.getAttribute('action');
        
        try {
            const response = await fetch(url, {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });
            
            const result = await response.json();
            
            if (result.success) {
                window.location.reload();
            } else {
                alert(result.message || 'Error saving food stock item');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Error saving food stock item');
        }
    });*/

    // Close modal when clicking the close button or outside the modal
    document.querySelector('.close').addEventListener('click', function() {
        document.getElementById('foodStockModal').style.display = 'none';
    });

    window.addEventListener('click', function(event) {
        const modal = document.getElementById('foodStockModal');
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    });

    document.querySelectorAll('.dashboard-btn-edit').forEach(button => {
        button.addEventListener('click', function() {
            const row = this.closest('tr');
            const itemId = row.dataset.itemId;
            
            editFoodStock(itemId);
        });
    });

});

function handleFormSubmit(e) {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);
    
    fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            form.closest('.modal').style.display = 'none';
            location.reload();
        } else {
            const errors = data.errors?.map(e => e.errorMessage).join('\n') || 'An error occurred';
            alert('Error: ' + errors);
        }
    })
    .catch(error => {
        console.error('Form submission error:', error);
        alert('An error occurred while submitting the form');
    });
}

function deleteFoodStock(id) {
    if (confirm('Are you sure you want to delete this item? This will also remove it from any meal plans.')) {
        fetch(`/Inventory/DeleteFoodStock/${id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const row = document.querySelector(`tr[data-item-id="${id}"]`);
                if (row) {
                    row.remove();
                }
            } else {
                alert(data.message || 'Failed to delete item');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while deleting the item');
        });
    }
}

function handleFoodStockSubmit(event) {
    event.preventDefault();
    const form = event.target;
    const formData = new FormData(form);

    fetch('/FoodStock/Create', {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (response.redirected) {
            window.location.href = response.url;
        } else {
            return response.json();
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('An error occurred while adding the food stock item');
    });
}
