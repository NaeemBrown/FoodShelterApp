document.addEventListener('DOMContentLoaded', function () {
    // Handle section navigation from sidebar
    document.querySelectorAll('.sidebar .nav-link').forEach(link => {
        if (link.getAttribute('href').startsWith('#')) {
            link.addEventListener('click', function (e) {
                e.preventDefault();
                const sectionId = this.getAttribute('href').substring(1);
                const section = document.getElementById(sectionId);
                if (section) {
                    section.scrollIntoView({ behavior: 'smooth' });
                }
            });
        }
    });

    // Setup section collapse/expand
    document.querySelectorAll('.section-header').forEach(header => {
        header.addEventListener('click', function () {
            const section = this.closest('.dashboard-section');
            const content = section.querySelector('.section-content');
            const toggleBtn = this.querySelector('.toggle-btn');

            content.classList.toggle('collapsed');
            toggleBtn.textContent = content.classList.contains('collapsed') ? '?' : '?';
        });
    });
});