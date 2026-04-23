$(document).ready(function () {
    console.log("Service details page loaded!");
    initializeProviderSelection();
    initializeBookingButton();
    initializeReviewsSection();
    initializeStickyBooking();
});

// ==========================================
// Provider Selection
// ==========================================
function initializeProviderSelection() {
    $('.select-provider-btn').on('click', function (e) {
        e.preventDefault(); // Prevent any default action

        // Get data from the parent card - use attr() to ensure we get the data
        const card = $(this).closest('.provider-selection-card');
        const providerId = card.attr('data-provider-id');
        const providerName = card.attr('data-provider-name');

        console.log('Provider clicked:', providerId, providerName); // Debugging
        console.log('Card element:', card); // Check if card is found

        // Validate that we have the necessary data
        if (!providerId || !providerName) {
            console.error('Missing provider data! ProviderId:', providerId, 'ProviderName:', providerName);
            alert('Error: Provider information not found. Please refresh the page.');
            return;
        }

        // Reset all cards
        $('.provider-selection-card').removeClass('provider-selected');
        $('.select-provider-btn')
            .text('Select Provider')
            .removeClass('btn-primary')
            .addClass('btn-outline-primary');

        // Highlight selected card
        card.addClass('provider-selected');
        $(this)
            .text('Selected')
            .removeClass('btn-outline-primary')
            .addClass('btn-primary');

        updateSelectedProvider(providerId, providerName);

        // Auto scroll on mobile
        if ($(window).width() < 992) {
            const stickyWrapper = $('.sticky-wrapper');
            if (stickyWrapper.length > 0) {
                $('html, body').animate({
                    scrollTop: stickyWrapper.offset().top - 100
                }, 500);
            }
        }
    });
}

// ==========================================
// Update Selected Provider
// ==========================================
function updateSelectedProvider(providerId, providerName) {
    console.log('Updating provider:', providerId, providerName); // Debugging

    $('#selectedProviderId').val(providerId);

    const displayHtml = `
        <div class="d-flex align-items-center">
            <i class="bi bi-person-check-fill text-success me-2"></i>
            <div>
                <strong>${providerName}</strong><br>
                <small class="text-muted">Provider selected</small>
            </div>
        </div>
    `;

    $('#selectedProviderDisplay')
        .removeClass('alert-info')
        .addClass('alert-success')
        .html(displayHtml)
        .hide()
        .fadeIn(300);

    // Enable booking button
    const serviceId = $('#bookNowBtn').data('service-id');
    const bookingUrl = `/Booking/Book?serviceId=${serviceId}&providerId=${providerId}`;

    console.log('Service ID:', serviceId, 'Provider ID:', providerId); // Debugging
    console.log('Booking URL will be:', bookingUrl); // Show the URL

    // Remove disabled class and set proper href
    $('#bookNowBtn')
        .removeClass('disabled')
        .attr('href', bookingUrl);

    console.log('Button href set to:', $('#bookNowBtn').attr('href')); // Verify href was set

    // Show success message
    if (typeof showToast === "function") {
        showToast('Provider selected successfully!', 'success');
    } else {
        console.log('✓ Provider selected successfully!');
    }
}

function initializeBookingButton() {
    $('#bookNowBtn').on('click', function (e) {
        const currentHref = $(this).attr('href');
        console.log('Book button clicked! Current href:', currentHref); // Debug
        console.log('Has disabled class?', $(this).hasClass('disabled')); // Debug

        if ($(this).hasClass('disabled')) {
            e.preventDefault();

            if (typeof showToast === "function") {
                showToast('Please select a provider first', 'warning');
            } else {
                alert('Please select a provider first');
            }

            $('html, body').animate({
                scrollTop: $('.provider-selection-card').first().offset().top - 100
            }, 500);

            $('.provider-selection-card').addClass('pulse-animation');
            setTimeout(() => {
                $('.provider-selection-card').removeClass('pulse-animation');
            }, 1000);
        } else {
            // Let the link navigate normally
            console.log('Navigating to:', currentHref);
        }
    });
}

function initializeReviewsSection() {
    const loadMoreBtn = $('#loadMoreReviews');
    if (loadMoreBtn.length === 0) {
        console.log('No load more reviews button found - skipping initialization');
        return;
    }

    loadMoreBtn.on('click', function () {
        const button = $(this);
        showLoading(button);
        setTimeout(() => {
            hideLoading(button);
            if (typeof showToast === "function") {
                showToast('All reviews loaded', 'info');
            }
            button.hide();
        }, 1000);
    });
    animateReviews();
}

// ==========================================
// Animate Reviews
// ==========================================
function animateReviews() {
    const reviewItems = document.querySelectorAll('.review-item');
    if (reviewItems.length === 0) {
        console.log('No review items found - skipping animation');
        return;
    }

    const observer = new IntersectionObserver(entries => {
        entries.forEach((entry, index) => {
            if (entry.isIntersecting) {
                setTimeout(() => {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateX(0)';
                }, index * 100);
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1 });

    reviewItems.forEach(item => {
        item.style.opacity = '0';
        item.style.transform = 'translateX(-20px)';
        item.style.transition = 'all 0.5s ease';
        observer.observe(item);
    });
}

// ==========================================
// Sticky Booking Sidebar
// ==========================================
function initializeStickyBooking() {
    if ($(window).width() >= 992) {
        const sidebar = $('.sticky-wrapper');
        if (sidebar.length === 0) {
            console.log('No sticky sidebar found - skipping');
            return;
        }

        const sidebarTop = sidebar.offset().top;
        $(window).on('scroll', function () {
            const scrollTop = $(window).scrollTop();
            sidebar.toggleClass('booking-fixed', scrollTop > sidebarTop - 100);
        });
    }
}

// ==========================================
// Helper functions
// ==========================================
function showLoading(element) {
    element.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2"></span>Loading...');
}

function hideLoading(element) {
    element.prop('disabled', false).html('Load More Reviews');
}

// ==========================================
// Styles injected
// ==========================================
if (!$('#service-detail-custom-css').length) {
    var customStyle = document.createElement('style');
    customStyle.id = 'service-detail-custom-css';
    customStyle.textContent = `
    @keyframes pulse {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.02); }
    }
    .pulse-animation {
        animation: pulse 0.5s ease-in-out 2;
    }
    .booking-fixed {
        position: fixed;
        top: 100px;
        width: inherit;
    }
    .provider-selected {
        border: 2px solid #667eea !important;
        background-color: rgba(102, 126, 234, 0.05) !important;
    }`;
    document.head.appendChild(customStyle);
}