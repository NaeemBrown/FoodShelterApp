document.addEventListener('DOMContentLoaded', function() {
    // Get modal elements
    const modal = document.getElementById('addMealModal');
    const closeBtn = modal.querySelector('.close');
    
    // Close button functionality
    if (closeBtn) {
        closeBtn.addEventListener('click', function() {
            modal.style.display = 'none';
        });
    }

    // Close on outside click
    window.addEventListener('click', function(event) {
        if (event.target === modal) {
            modal.style.display = 'none';
        }
    });

    // Add ingredient field functionality
    window.loadIngredientField = function() {
        const container = document.getElementById('ingredientsList');
        container.innerHTML = ''; // Clear existing fields
        addIngredientField();
    }

    window.addIngredientField = function() {
        const container = document.getElementById('ingredientsList');
        const template = document.getElementById('ingredientTemplate');
        const div = document.createElement('div');
        div.innerHTML = template.innerHTML;
        container.appendChild(div);
    }

    window.openAddMealModal = function() {
        modal.style.display = 'block';
        loadIngredientField();
    }

    // Form submission
    const form = document.getElementById('addMealForm');
    if (form) {
        form.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const formData = {
                name: document.getElementById('mealName').value,
                servings: parseInt(document.getElementById('servings').value),
                ingredients: []
            };

            const ingredients = document.querySelectorAll('.ingredient-row');
            ingredients.forEach(row => {
                formData.ingredients.push({
                    foodStockId: parseInt(row.querySelector('select').value),
                    quantityPerServing: parseFloat(row.querySelector('input[type="number"]').value)
                });
            });

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const response = await fetch('/MealPrep/AddMeal', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(formData)
                });

                const result = await response.json();
                if (result.success) {
                    modal.style.display = 'none';
                    form.reset();
                    location.reload();
                } else {
                    alert(result.message || 'Failed to add meal plan');
                }
            } catch (error) {
                console.error('Error:', error);
                alert('Error adding meal plan');
            }
        });
    }
});

async function prepareMeal(id) {
    try {
        const response = await fetch(`/MealPrep/PrepareMeal/${id}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        const result = await response.json();
        if (result.success) {
            location.reload();
        } else {
            alert(result.message || 'Failed to prepare meal');
        }
    } catch (error) {
        alert('Error preparing meal');
    }
}

function deleteMeal(id) {
    if (confirm('Are you sure you want to delete this meal plan?')) {
        fetch(`/MealPrep/DeleteMeal/${id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const mealCard = document.querySelector(`.meal-plan-card[data-meal-id="${id}"]`);
                if (mealCard) {
                    mealCard.remove();
                }
            } else {
                alert(data.message || 'Failed to delete meal plan');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while deleting the meal plan');
        });
    }
}
