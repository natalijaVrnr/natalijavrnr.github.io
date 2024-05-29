// Function to toggle dark mode
function toggleDarkMode() {
    // Toggle dark mode class on body
    document.body.classList.toggle('dark-mode');
    
    // Toggle dark mode class on paragraphs
    var paragraphs = document.querySelectorAll('p');
    paragraphs.forEach(function(p) {
        p.classList.toggle('dark-mode');
    });
    
    // Toggle dark mode class on headings
    var headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
    headings.forEach(function(h) {
        h.classList.toggle('dark-mode');
    });
    
    // Toggle dark mode class on buttons
    var buttons = document.querySelectorAll('button');
    buttons.forEach(function(button) {
        button.classList.toggle('dark-mode');
    });
    
    // Toggle dark mode class on links
    var links = document.querySelectorAll('a');
    links.forEach(function(link) {
        link.classList.toggle('dark-mode');
    });

    // Toggle dark mode class on images
    var images = document.querySelectorAll('img');
    images.forEach(function(img) {
        img.classList.toggle('dark-mode'); 
    });
    
    // Toggle dark mode class on ordered lists
    var ols = document.querySelectorAll('ol');
    ols.forEach(function(ol) {
        ol.classList.toggle('dark-mode'); 
    });

    // Update dark mode state in local storage
    var darkModeState = document.body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', darkModeState);
}

// Function to initialize dark mode based on local storage state
function initializeDarkMode() {
    var darkModeState = localStorage.getItem('darkMode');
    if (darkModeState === 'true') {
        // Apply dark mode if it was enabled
        document.body.classList.add('dark-mode');

        // Apply dark mode to all paragraphs
        var paragraphs = document.querySelectorAll('p');
        paragraphs.forEach(function(p) {
            p.classList.add('dark-mode');
        });
        
        // Apply dark mode to all headings
        var headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
        headings.forEach(function(h) {
            h.classList.add('dark-mode');
        });
        
        // Apply dark mode to all buttons
        var buttons = document.querySelectorAll('button');
        buttons.forEach(function(button) {
            button.classList.add('dark-mode');
        });
        
        // Apply dark mode to all links
        var links = document.querySelectorAll('a');
        links.forEach(function(link) {
            link.classList.add('dark-mode');
        });

        // Apply dark mode to all images
        var images = document.querySelectorAll('img');
        images.forEach(function(img) {
            img.classList.add('dark-mode');
        });
        
        // Apply dark mode to all ordered lists
        var ols = document.querySelectorAll('ol');
        ols.forEach(function(ol) {
            ol.classList.add('dark-mode');
        });
    }
}

// Add event listener to toggle button
document.getElementById('toggle-mode').addEventListener('click', function() {
    toggleDarkMode();
});

// Initialize dark mode state on page load
initializeDarkMode();