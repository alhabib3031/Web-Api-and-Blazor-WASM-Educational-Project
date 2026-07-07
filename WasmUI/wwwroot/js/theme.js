window.toggleDarkClass = function () {
    document.documentElement.classList.toggle('dark');
};

window.isDarkMode = function () {
    return document.documentElement.classList.contains('dark');
};
