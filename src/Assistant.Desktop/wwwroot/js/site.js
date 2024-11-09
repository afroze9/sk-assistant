// Add this to your wwwroot/js/site.js file
window.scrollToBottom = (element) => {
    setTimeout(() => {
        element.scrollTop = element.scrollHeight;
    }, 500)
};