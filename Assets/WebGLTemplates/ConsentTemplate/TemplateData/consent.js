// Consent form logic
const consentCheckbox = document.getElementById('consent-checkbox');
const startButton = document.getElementById('start-button');
const consentContainer = document.getElementById('consent-container');
const unityContainer = document.getElementById('unity-container');

// Future use checkboxes
const futureUseSame = document.getElementById('future-use-same');
const futureUseOthers = document.getElementById('future-use-others');
const futureUseNone = document.getElementById('future-use-none');

// Enable start button only when main consent checkbox is checked and at least one future use option is selected
function checkFormValidity() {
    const consentGiven = consentCheckbox.checked;
    const futureUseSelected = futureUseSame.checked || futureUseOthers.checked || futureUseNone.checked;

    startButton.disabled = !(consentGiven && futureUseSelected);
}

consentCheckbox.addEventListener('change', checkFormValidity);
futureUseSame.addEventListener('change', checkFormValidity);
futureUseOthers.addEventListener('change', checkFormValidity);
futureUseNone.addEventListener('change', checkFormValidity);

// Track if user has scrolled through the consent form
let hasScrolledToBottom = false;
const consentForm = document.getElementById('consent-form');

consentForm.addEventListener('scroll', function() {
    const scrollTop = consentForm.scrollTop;
    const scrollHeight = consentForm.scrollHeight;
    const clientHeight = consentForm.clientHeight;

    // Check if user has scrolled near the bottom (within 50px)
    if (scrollTop + clientHeight >= scrollHeight - 50) {
        hasScrolledToBottom = true;
    }
});

// Start button click handler
startButton.addEventListener('click', function() {
    if (!consentCheckbox.checked) {
        alert('Please check the consent box to continue.');
        return;
    }

    // Check that at least one future use option is selected
    const futureUseSelected = futureUseSame.checked || futureUseOthers.checked || futureUseNone.checked;
    if (!futureUseSelected) {
        alert('Please select at least one option for future use of your information.');
        return;
    }

    // Record consent timestamp and choices
    const consentTimestamp = new Date().toISOString();
    console.log('Consent given at:', consentTimestamp);

    // Collect future use preferences
    const futureUsePreferences = {
        sameResearchers: futureUseSame.checked,
        otherResearchers: futureUseOthers.checked,
        noFutureUse: futureUseNone.checked
    };

    // Store consent in localStorage for record keeping
    localStorage.setItem('consentGiven', consentTimestamp);
    localStorage.setItem('consentScrolledToBottom', hasScrolledToBottom);
    localStorage.setItem('futureUsePreferences', JSON.stringify(futureUsePreferences));

    console.log('Future use preferences:', futureUsePreferences);

    // Hide consent form with fade out animation
    consentContainer.classList.add('fade-out');

    // After animation completes, hide consent and show Unity game
    setTimeout(function() {
        consentContainer.style.display = 'none';
        unityContainer.style.display = 'block';

        // Initialize Unity game
        loadUnityGame();
    }, 500);
});

// Function to load Unity WebGL game
function loadUnityGame() {
    const buildUrl = "Build";
    const loaderUrl = buildUrl + "/Build.loader.js";
    const config = {
        dataUrl: buildUrl + "/Build.data",
        frameworkUrl: buildUrl + "/Build.framework.js",
        codeUrl: buildUrl + "/Build.wasm",
        streamingAssetsUrl: "StreamingAssets",
        companyName: "DefaultCompany",
        productName: "Robotics_Trust_2025",
        productVersion: "1.0",
    };

    const canvas = document.getElementById('unity-canvas');
    const loadingBar = document.getElementById('unity-loading-bar');
    const progressBarFull = document.getElementById('unity-progress-bar-full');
    const warningBanner = document.getElementById('unity-warning');

    // Show loading bar
    loadingBar.style.display = 'block';

    // Create script element to load Unity loader
    const script = document.createElement('script');
    script.src = loaderUrl;
    script.onload = function() {
        createUnityInstance(canvas, config, function(progress) {
            progressBarFull.style.width = 100 * progress + '%';
        }).then(function(unityInstance) {
            loadingBar.style.display = 'none';

            // Send consent data to Unity (if needed)
            unityInstance.SendMessage('ConsentTracker', 'SetConsentData', localStorage.getItem('consentGiven'));
        }).catch(function(message) {
            alert('Failed to load game: ' + message);
        });
    };
    script.onerror = function() {
        warningBanner.innerHTML = 'Failed to load Unity game. Please check that the Build files exist.';
        warningBanner.style.display = 'block';
        loadingBar.style.display = 'none';
    };
    document.body.appendChild(script);
}

// Optional: Check if consent was already given (for returning users)
window.addEventListener('load', function() {
    const previousConsent = localStorage.getItem('consentGiven');
    if (previousConsent) {
        console.log('Previous consent found from:', previousConsent);
        // You can choose to auto-skip the consent form here if desired
        // For research purposes, you may want to show it every time
    }
});
