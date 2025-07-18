document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.querySelector('.sidebar');
    const content = document.querySelector('.dashboard-content');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            sidebar.classList.toggle('collapsed');
            if (sidebar.classList.contains('collapsed')) {
                content.style.marginLeft = '60px';
                sidebar.style.width = '60px';
            } else {
                content.style.marginLeft = '250px';
                sidebar.style.width = '250px';
            }
        });
    }

    function handleResize() {
        if (window.innerWidth < 768) {
            sidebar.classList.add('collapsed');
            content.style.marginLeft = '60px';
            sidebar.style.width = '60px';
        } else {
            sidebar.classList.remove('collapsed');
            content.style.marginLeft = '250px';
            sidebar.style.width = '250px';
        }
    }

    window.addEventListener('resize', handleResize);
    handleResize(); // Initial check
});