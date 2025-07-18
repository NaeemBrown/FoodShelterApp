// wwwroot/js/modals.js
document.addEventListener('DOMContentLoaded', function () {
    // Setup modal triggers
    setupModal('addFoodStockBtn', 'foodStockModal');
    setupModal('addShelterLocationBtn', 'shelterLocationModal');
    setupModal('addNoteBtn', 'noteModal');
    setupModal('addVolunteerBtn', 'volunteerModal');
    setupModal('addBudgetEntryBtn', 'budgetModal');
    setupModal('addDonationBtn', 'donationModal');
});

function setupModal(buttonId, modalId) {
    const button = document.getElementById(buttonId);
    const modal = document.getElementById(modalId);
    if (!button || !modal) return;

    const closeBtn = modal.querySelector('.close');

    button.onclick = function () {
        modal.style.display = 'block';
    }

    if (closeBtn) {
        closeBtn.onclick = function () {
            modal.style.display = 'none';
        }
    }

    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    }
}