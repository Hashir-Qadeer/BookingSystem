// ==========================================
// Site-wide JavaScript
// ==========================================

$(document).ready(function () {
    console.log("BookingSystem loaded successfully!");

    // Initialize tooltips
    initializeTooltips();

    // Smooth scroll for anchor links
    initializeSmoothScroll();

    // Navbar scroll effect
    initializeNavbarScroll();

    // Animate elements on scroll
    initializeScrollAnimations();

    // Form validation
    initializeFormValidation();
});

// ==========================================
// Initialize Bootstrap Tooltips
// ==========================================
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// ==========================================
// Smooth Scroll for Anchor Links
// ==========================================
function initializeSmoothScroll() {
    $('a[href^="#"]').on('click', function (e) {
        var target = $(this.getAttribute('href'));
        if (target.length) {
            e.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 80
            }, 1000);
        }
    });
}

// ==========================================
// Navbar Scroll Effect
// ==========================================
function initializeNavbarScroll() {
    $(window).scroll(function () {
        if ($(window).scrollTop() > 50) {
            $('.navbar').addClass('navbar-scrolled');
        } else {
            $('.navbar').removeClass('navbar-scrolled');
        }
    });
}

// ==========================================
// Scroll Animations
// ==========================================
function initializeScrollAnimations() {
    // Intersection Observer for fade-in animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-fade-in');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    // Observe all cards and sections
    document.querySelectorAll('.card, .stat-card').forEach(el => {
        observer.observe(el);
    });
}

// ==========================================
// Form Validation
// ==========================================
function initializeFormValidation() {
    // Bootstrap form validation
    var forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
}

// ==========================================
// Show Loading Spinner
// ==========================================
function showLoading(buttonElement) {
    var originalText = buttonElement.html();
    buttonElement.data('original-text', originalText);
    buttonElement.html('<span class="spinner-border spinner-border-sm me-2"></span>Loading...');
    buttonElement.prop('disabled', true);
}

// ==========================================
// Hide Loading Spinner
// ==========================================
function hideLoading(buttonElement) {
    var originalText = buttonElement.data('original-text');
    buttonElement.html(originalText);
    buttonElement.prop('disabled', false);
}

// ==========================================
// Show Toast Notification
// ==========================================
function showToast(message, type = 'success') {
    var toastHtml = `
        <div class="toast align-items-center text-white bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;

    var toastContainer = $('#toast-container');
    if (toastContainer.length === 0) {
        $('body').append('<div id="toast-container" class="toast-container position-fixed top-0 end-0 p-3"></div>');
        toastContainer = $('#toast-container');
    }

    var toastElement = $(toastHtml);
    toastContainer.append(toastElement);

    var toast = new bootstrap.Toast(toastElement[0], {
        autohide: true,
        delay: 3000
    });
    toast.show();

    toastElement.on('hidden.bs.toast', function () {
        $(this).remove();
    });
}

// ==========================================
// Confirm Dialog
// ==========================================
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// ==========================================
// Format Date
// ==========================================
function formatDate(dateString) {
    var date = new Date(dateString);
    var options = { year: 'numeric', month: 'long', day: 'numeric' };
    return date.toLocaleDateString('en-US', options);
}

// ==========================================
// Format Time
// ==========================================
function formatTime(timeString) {
    var time = new Date('1970-01-01T' + timeString);
    return time.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
}

// ==========================================
// AJAX Helper Function
// ==========================================
function ajaxRequest(url, method, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: method,
        data: data,
        success: function (response) {
            if (successCallback) {
                successCallback(response);
            }
        },
        error: function (xhr, status, error) {
            if (errorCallback) {
                errorCallback(xhr, status, error);
            } else {
                showToast('An error occurred. Please try again.', 'danger');
            }
        }
    });
}

// ==========================================
// Debounce Function (for search inputs)
// ==========================================
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// ==========================================
// Add CSS Animation Class
// ==========================================
const style = document.createElement('style');
style.textContent = `
    .animate-fade-in {
        animation: fadeInUp 0.6s ease-out;
    }
    
    @keyframes fadeInUp {
        from {
            opacity: 0;
            transform: translateY(30px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
`;
document.head.appendChild(style);