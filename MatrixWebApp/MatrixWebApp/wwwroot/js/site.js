// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleUserDropdown() {
    const dropdown = document.getElementById('userDropdown');
    dropdown.classList.toggle('hidden');
}

document.addEventListener('click', function (event) {
    const userContainer = document.querySelector('.user-container');
    const isClickInside = userContainer?.contains(event.target);
    const dropdown = document.querySelector('#userDropdown');

    if (!isClickInside && dropdown && !dropdown.classList.contains('hidden')) {
        dropdown.classList.add('hidden');
    }
});