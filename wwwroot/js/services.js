// ==========================================
// Services Page JavaScript
// ==========================================

$(document).ready(function () {
    console.log("Services page loaded!");

    // Initialize all features
    initializePriceRange();
    initializeSearchSuggestions();
    initializeViewToggle();
    initializeAutoSubmit();
    initializeResetFilters();
});

// ==========================================
// Price Range Slider
// ==========================================
function initializePriceRange() {
    const minPriceRange = $('#minPriceRange');
    const maxPriceRange = $('#maxPriceRange');
    const priceDisplay = $('#priceRangeDisplay');

    function updatePriceDisplay() {
        const minPrice = parseInt(minPriceRange.val());
        const maxPrice = parseInt(maxPriceRange.val());

        // Ensure min doesn't exceed max
        if (minPrice > maxPrice) {
            minPriceRange.val(maxPrice);
        }

        priceDisplay.text(`$${minPriceRange.val()} - $${maxPriceRange.val()}`);
    }

    minPriceRange.on('input', updatePriceDisplay);
    maxPriceRange.on('input', updatePriceDisplay);

    // Auto-submit on price change after delay
    let priceTimeout;
    minPriceRange.on('change', function () {
        clearTimeout(priceTimeout);
        priceTimeout = setTimeout(() => {
            $('#filterForm').submit();
        }, 500);
    });

    maxPriceRange.on('change', function () {
        clearTimeout(priceTimeout);
        priceTimeout = setTimeout(() => {
            $('#filterForm').submit();
        }, 500);
    });
}

// ==========================================
// Search Suggestions (AJAX)
// ==========================================
function initializeSearchSuggestions() {
    const searchInput = $('#searchInput');
    const suggestionsContainer = $('#searchSuggestions');
    let searchTimeout;

    searchInput.on('input', function () {
        const query = $(this).val().trim();

        clearTimeout(searchTimeout);

        if (query.length < 2) {
            suggestionsContainer.hide().html('');
            return;
        }

        searchTimeout = setTimeout(() => {
            // AJAX call to get suggestions
            $.ajax({
                url: '/Services/SearchServices',
                type: 'GET',
                data: { query: query },
                success: function (results) {
                    displaySearchSuggestions(results);
                },
                error: function () {
                    console.error('Failed to load suggestions');
                }
            });
        }, 300);
    });

    // Hide suggestions when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('#searchInput, #searchSuggestions').length) {
            suggestionsContainer.hide();
        }
    });
}

function displaySearchSuggestions(results) {
    const suggestionsContainer = $('#searchSuggestions');

    if (results.length === 0) {
        suggestionsContainer.hide();
        return;
    }

    let html = '<ul class="list-group shadow-sm">';
    results.forEach(result => {
        html += `
            <li class="list-group-item list-group-item-action suggestion-item" 
                data-service-id="${result.id}">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <strong>${result.name}</strong>
                        <br>
                        <small class="text-muted">${result.category}</small>
                    </div>
                    <span class="badge bg-primary">$${result.price}</span>
                </div>
            </li>
        `;
    });
    html += '</ul>';

    suggestionsContainer.html(html).show();

    // Handle suggestion click
    $('.suggestion-item').on('click', function () {
        const serviceId = $(this).data('service-id');
        window.location.href = `/Services/Details/${serviceId}`;
    });
}

// ==========================================
// View Toggle (Grid/List)
// ==========================================
function initializeViewToggle() {
    const gridViewBtn = $('#gridView');
    const listViewBtn = $('#listView');
    const servicesContainer = $('#servicesContainer');

    gridViewBtn.on('click', function () {
        $(this).addClass('active');
        listViewBtn.removeClass('active');
        servicesContainer.removeClass('list-view').addClass('row g-4');

        // Update column classes
        $('.service-item').removeClass('col-12').addClass('col-lg-4 col-md-6');
    });

    listViewBtn.on('click', function () {
        $(this).addClass('active');
        gridViewBtn.removeClass('active');
        servicesContainer.addClass('list-view');

        // Update column classes for list view
        $('.service-item').removeClass('col-lg-4 col-md-6').addClass('col-12');
    });
}

// ==========================================
// Auto-submit filters on change
// ==========================================
function initializeAutoSubmit() {
    $('#categoryFilter, #sortFilter').on('change', function () {
        showLoading($('#filterForm button[type="submit"]'));
        $('#filterForm').submit();
    });
}

// ==========================================
// Reset Filters
// ==========================================
function initializeResetFilters() {
    $('#resetFilters, #clearFiltersBtn').on('click', function () {
        window.location.href = '/Services/Index';
    });
}

// ==========================================
// Smooth Card Animations
// ==========================================
function animateServiceCards() {
    const cards = document.querySelectorAll('.service-card');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry, index) => {
            if (entry.isIntersecting) {
                setTimeout(() => {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, index * 100);
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1 });

    cards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.5s ease';
        observer.observe(card);
    });
}

// Call animation on page load
animateServiceCards();

// ==========================================
// Filter Statistics
// ==========================================
function updateFilterStats() {
    const totalServices = $('.service-item').length;
    console.log(`Displaying ${totalServices} services`);
}

updateFilterStats();