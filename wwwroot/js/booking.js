// ==========================================
// Booking Page JavaScript - Clean Version
// ==========================================

(function () {
    'use strict';

    $(document).ready(function () {
        console.log("Booking page initialized!");

        // Initialize only if styles haven't been added
        initializeBookingPageStyles();

        // Initialize all components
        setupDatePicker();
        setupTimeSlotSelection();
        setupFormSubmission();
    });

    // ==========================================
    // Initialize Styles
    // ==========================================
    function initializeBookingPageStyles() {
        if (document.getElementById('booking-styles-v2')) {
            console.log('Styles already loaded');
            return;
        }

        const styleElement = document.createElement('style');
        styleElement.id = 'booking-styles-v2';
        styleElement.textContent = `
            .date-card {
                text-align: center;
                padding: 1rem 0.5rem;
                border: 2px solid #e9ecef;
                border-radius: 12px;
                cursor: pointer;
                transition: all 0.3s ease;
                background: white;
            }
            .date-card:hover {
                border-color: #667eea;
                transform: translateY(-3px);
                box-shadow: 0 4px 12px rgba(102, 126, 234, 0.15);
            }
            .date-card.selected {
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                border-color: #667eea;
                color: white;
                transform: translateY(-3px);
                box-shadow: 0 6px 16px rgba(102, 126, 234, 0.3);
            }
            .date-day {
                font-size: 0.75rem;
                font-weight: 600;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                opacity: 0.9;
            }
            .date-number {
                font-size: 1.75rem;
                font-weight: bold;
                margin: 0.25rem 0;
                line-height: 1;
            }
            .date-month {
                font-size: 0.75rem;
                text-transform: uppercase;
                opacity: 0.8;
            }
            .time-slot-btn {
                width: 100%;
                padding: 0.875rem 0.5rem;
                border: 2px solid #e9ecef;
                border-radius: 10px;
                background: white;
                cursor: pointer;
                transition: all 0.3s ease;
                font-weight: 600;
                font-size: 0.95rem;
            }
            .time-slot-btn:hover:not(.disabled) {
                border-color: #667eea;
                background: rgba(102, 126, 234, 0.08);
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(102, 126, 234, 0.15);
            }
            .time-slot-btn.selected {
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
                border-color: #667eea !important;
                color: white !important;
                transform: translateY(-2px);
                box-shadow: 0 6px 16px rgba(102, 126, 234, 0.3);
            }
            .time-slot-btn.disabled {
                opacity: 0.4;
                cursor: not-allowed;
                background: #f8f9fa;
                color: #6c757d;
            }
            .time-slot-btn.disabled:hover {
                transform: none;
                box-shadow: none;
            }
            .booking-service-img {
                width: 80px;
                height: 80px;
                object-fit: cover;
                flex-shrink: 0;
                border-radius: 8px;
            }
            .booking-provider-img {
                width: 60px;
                height: 60px;
                object-fit: cover;
                flex-shrink: 0;
            }
            .progress-step {
                position: relative;
            }
            .step-icon {
                width: 60px;
                height: 60px;
                margin: 0 auto;
                border-radius: 50%;
                background: #e9ecef;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 1.5rem;
                color: #6c757d;
                transition: all 0.3s ease;
            }
            .progress-step.completed .step-icon {
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
            }
            .progress-step.active .step-icon {
                background: white;
                border: 3px solid #667eea;
                color: #667eea;
                animation: pulse-effect 2s infinite;
            }
            @keyframes pulse-effect {
                0%, 100% { 
                    box-shadow: 0 0 0 0 rgba(102, 126, 234, 0.4); 
                }
                50% { 
                    box-shadow: 0 0 0 10px rgba(102, 126, 234, 0); 
                }
            }
            .date-picker-container {
                background: #f8f9fa;
                padding: 1.5rem;
                border-radius: 12px;
            }
            @media (max-width: 768px) {
                .date-card {
                    padding: 0.75rem 0.25rem;
                }
                .date-number {
                    font-size: 1.5rem;
                }
                .time-slot-btn {
                    padding: 0.75rem 0.5rem;
                    font-size: 0.875rem;
                }
                .step-icon {
                    width: 50px;
                    height: 50px;
                    font-size: 1.25rem;
                }
            }
        `;
        document.head.appendChild(styleElement);
        console.log('Booking styles loaded successfully');
    }

    // ==========================================
    // Date Picker Setup
    // ==========================================
    function setupDatePicker() {
        $('.date-card').off('click').on('click', function () {
            const selectedDate = $(this).data('date');
            console.log('Date selected:', selectedDate);

            // Update UI
            $('.date-card').removeClass('selected');
            $(this).addClass('selected');

            // Update form values
            $('#selectedDateInput').val(selectedDate);
            $('#selectedDateDisplay').text(formatDateDisplay(selectedDate));

            // Load time slots
            loadTimeSlotsForDate(selectedDate);

            // Reset selection
            resetTimeSlotSelection();
        });
    }

    // ==========================================
    // Load Time Slots
    // ==========================================
    function loadTimeSlotsForDate(date) {
        const providerId = $('input[name="providerId"]').val();
        console.log('Loading slots for provider:', providerId, 'on date:', date);

        // Show loading state
        $('#timeSlotsContainer').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="text-muted mt-2">Loading available time slots...</p>
            </div>
        `);

        // AJAX call
        $.ajax({
            url: '/Booking/GetTimeSlots',
            type: 'GET',
            data: { providerId: providerId, date: date },
            success: function (slots) {
                console.log('Slots received:', slots);
                renderTimeSlots(slots);
            },
            error: function (xhr, status, error) {
                console.error('Error loading slots:', error);
                $('#timeSlotsContainer').html(`
                    <div class="alert alert-danger">
                        <i class="bi bi-exclamation-triangle"></i>
                        Failed to load time slots. Please try again.
                    </div>
                `);
            }
        });
    }

    // ==========================================
    // Render Time Slots
    // ==========================================
    function renderTimeSlots(slots) {
        if (!slots || slots.length === 0) {
            $('#timeSlotsContainer').html(`
                <div class="alert alert-warning">
                    <i class="bi bi-info-circle"></i>
                    No available time slots for this date.
                </div>
            `);
            return;
        }

        let html = '<div class="row g-2">';

        slots.forEach(function (slot) {
            const isDisabled = !slot.isAvailable;
            const disabledAttr = isDisabled ? 'disabled' : '';
            const disabledClass = isDisabled ? 'disabled' : '';

            html += `
                <div class="col-md-3 col-6">
                    <button type="button" 
                            class="time-slot-btn ${disabledClass}" 
                            data-start-time="${slot.startTime}"
                            ${disabledAttr}>
                        ${formatTimeDisplay(slot.startTime)}
                    </button>
                </div>
            `;
        });

        html += '</div>';
        $('#timeSlotsContainer').html(html);

        // Attach event handlers to new buttons
        setupTimeSlotSelection();
    }

    // ==========================================
    // Time Slot Selection Setup
    // ==========================================
    function setupTimeSlotSelection() {
        // Remove existing handlers to prevent duplicates
        $('.time-slot-btn').off('click');

        // Attach new handlers
        $('.time-slot-btn:not(.disabled)').on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const startTime = $(this).attr('data-start-time');
            console.log('Time slot clicked:', startTime);

            if (!startTime) {
                console.error('No start time found!');
                return;
            }

            // Update UI
            $('.time-slot-btn').removeClass('selected');
            $(this).addClass('selected');

            // Update form
            $('#selectedStartTime').val(startTime);
            console.log('Updated hidden input to:', $('#selectedStartTime').val());

            // Update display
            updateTimeDisplay(startTime);

            // Enable button
            $('#confirmBookingBtn').prop('disabled', false).removeClass('disabled');
            console.log('Confirm button enabled');
        });

        console.log('Attached handlers to', $('.time-slot-btn:not(.disabled)').length, 'time slots');
    }

    // ==========================================
    // Update Time Display
    // ==========================================
    function updateTimeDisplay(startTime) {
        const selectedDate = $('#selectedDateInput').val();
        const displayElement = $('#selectedTimeDisplay');

        if (startTime) {
            displayElement
                .removeClass('alert-warning')
                .addClass('alert-success')
                .html(`
                    <i class="bi bi-check-circle"></i>
                    <strong>${formatDateDisplay(selectedDate)} at ${formatTimeDisplay(startTime)}</strong>
                `);
        } else {
            displayElement
                .removeClass('alert-success')
                .addClass('alert-warning')
                .html(`
                    <i class="bi bi-exclamation-triangle"></i>
                    Please select a time slot
                `);
        }
    }

    // ==========================================
    // Reset Time Slot Selection
    // ==========================================
    function resetTimeSlotSelection() {
        $('.time-slot-btn').removeClass('selected');
        $('#selectedStartTime').val('');
        updateTimeDisplay(null);
        $('#confirmBookingBtn').prop('disabled', true).addClass('disabled');
    }

    // ==========================================
    // Form Submission Setup
    // ==========================================
    function setupFormSubmission() {
        $('#bookingForm').on('submit', function (e) {
            const startTime = $('#selectedStartTime').val();
            console.log('Form submitting with startTime:', startTime);

            if (!startTime) {
                e.preventDefault();
                alert('Please select a time slot');

                $('html, body').animate({
                    scrollTop: $('#timeSlotsContainer').offset().top - 100
                }, 500);

                return false;
            }

            // Show loading
            $('#confirmBookingBtn')
                .prop('disabled', true)
                .html('<span class="spinner-border spinner-border-sm me-2"></span>Processing...');
        });
    }

    // ==========================================
    // Format Date for Display
    // ==========================================
    function formatDateDisplay(dateString) {
        const date = new Date(dateString + 'T00:00:00');
        const options = { month: 'short', day: 'numeric', year: 'numeric' };
        return date.toLocaleDateString('en-US', options);
    }

    // ==========================================
    // Format Time for Display
    // ==========================================
    function formatTimeDisplay(timeString) {
        const parts = timeString.split(':');
        let hours = parseInt(parts[0]);
        const minutes = parts[1];
        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12 || 12;
        return `${hours}:${minutes} ${ampm}`;
    }

})();