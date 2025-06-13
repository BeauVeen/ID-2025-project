document.addEventListener("DOMContentLoaded", function () {
    const collapseEl = document.getElementById("collapseProducts");

    // Check localStorage
    if (localStorage.getItem("productsDropdownOpen") === "true") {
        collapseEl.classList.add("show");
    }

    // Bootstrap Collapse events
    collapseEl.addEventListener("shown.bs.collapse", function () {
        localStorage.setItem("productsDropdownOpen", "true");
    });
    collapseEl.addEventListener("hidden.bs.collapse", function () {
        localStorage.setItem("productsDropdownOpen", "false");
    });
});