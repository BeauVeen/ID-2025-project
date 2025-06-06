document.addEventListener('DOMContentLoaded', function () {
    var currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.admin-sidebar-link').forEach(function(link) {
        var href = link.getAttribute('href');
        if (!href) return;
        var linkPath = href.split('?')[0].toLowerCase();
        if (currentPath.endsWith(linkPath)) {
            link.classList.add('active');
        }
    });
});