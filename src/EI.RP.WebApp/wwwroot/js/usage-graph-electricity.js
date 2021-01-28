$(function() {

    // Init Moment.js
    moment.updateLocale('en', {
        week: {
            dow: 1 // Week starts on Mondays
        }
    });

    // Min + Max Date - backend service should provide this
    window.minDate = moment($('meta[name="min-graph-date"]').attr("content"));
    window.maxDate = moment($('meta[name="max-graph-date"]').attr("content"));

    // App State Object (defaults)
    window.appState = {
        "period": "week",
        "date": maxDate.clone().startOf('isoWeek').format('YYYY-MM-DD'),
        "comparison": null,
        "units": "eur",
        "eiChart": null,
        "totals": [0, 0, 0, 0],
        "diffs": [0, 0]
    };

    initAppStateFromUrl();
    updateGraph();

    // Period Switcher
    $('.period-switcher .nav-item a.nav-link').on('click', function (ev) {
        ev.preventDefault();
        appState.period = $(this).data('period');
        appState.date = covertDateToPeriod(appState.date, appState.period);
        appState.comparison = null;
        $('.ei-switch').find('.comparison-period').text(appState.period + 's');
        updateUrlFromAppState();
        initAppStateFromUrl();
        updateGraph();
    });

    // Switch Euro / kWh
    $('.graph-value-switch input').on('change', function(ev) {
        var total = 0;
        if (this.id === 'value-eur') {
            appState.units = 'eur';
        } else if ( this.id === 'value-kwh' ) {
            appState.units = 'kwh';
            total = 1;
        }
        appState.eiChart.series[0].setVisible(appState.units === 'eur', false);
        appState.eiChart.series[1].setVisible(appState.units === 'kwh', false);
        appState.eiChart.series[2].setVisible(appState.units === 'eur' && appState.comparison, false);
        appState.eiChart.series[3].setVisible(appState.units === 'kwh' && appState.comparison, false);
        appState.eiChart.redraw();
        $('.graph-sum').find('.amount').attr('data', this.id).find('.value').text(appState.totals[total]);
        updateTotals();
        updateUrlFromAppState();
    });

    // Change period range
    $('.period-nav button').on('click', function(ev) {
        ev.preventDefault();
        var period = appState.period;
        var direction = $(this).data('mode');
        var currentRange = $(this).closest('.period-nav').find('.selected-range').text();
        var filterElem = $('#pills-' + period + ' .period-nav .selected-range');
        switch (period) {
            case "day":
                switchDay(currentRange, direction, filterElem);
                break;
            case "week":
                switchWeek(currentRange, direction, filterElem);
                break;
            case "month":
                switchMonth(currentRange, direction, filterElem);
                break;
            case "year":
                switchYear(currentRange, direction, filterElem);
                break;
            default:
                break;
        }
    });

    // Comparison modal
    $('#compareSwitch').on('change', function() {
        if ($(this).prop('checked')) {
            $('#comparisonModal').modal('toggle');
        } else {
            appState.comparison = null;
            $('.graph-wrapper .graph-sum .title').html('Total');
            updateUrlFromAppState();
            if ($("main.usage").hasClass("comparison")) {
                $("main.usage").removeClass("comparison");
                updateGraph();
            }
        }
    });

    $('#comparisonModal').on('show.bs.modal', function(ev) {
        $(this).find('.modal-title .comparison-period').text(appState.period + 's');
        initDatePicker($('#eidatetimepicker-comparison'), 'comparison-modal');
    }).on('hidden.bs.modal', function(ev) {
        destroyDatePicker($('#eidatetimepicker-comparison'));
        if (!appState.comparison) $('#compareSwitch').prop("checked", false).change();
    }).on('click', '#compareNow', function(ev) {
        ev.stopPropagation();
        var dates = $('#eidatetimepicker-comparison').datetimepicker('getDates');
        appState.date = dates[0].format("YYYY-MM-DD");
        appState.comparison = dates[1].format("YYYY-MM-DD");
        updateUrlFromAppState();
        initAppStateFromUrl();
        loadComparisonData();
    });

    // Show DatePicker when date is clicked
    $('.selected-range').on('click', function(ev) {
        ev.preventDefault();
        initDatePicker($('#eidatetimepicker'), undefined);
    });

    // Show DatePicker when comprison date is clicked
    $('.selected-range-compare').on('click', function(ev) {
        ev.preventDefault();
        initDatePicker($('#eidatetimepicker'), 'comparison');
    });

    // Listen to DOM mutations
    $(document).on("DOMNodeInserted", function(ev) {
        // Add .active / .disabled class to table row "week" period (not possible with pure CSS)
        $('.ei-datepicker').find('td.day.active').parent().addClass('active');
        $('.ei-datepicker').find('td.day.index-1').parent().addClass('index-1');
    });

});


// FUNCTIONS -----------------------------------------------------------------------------------------------

// Prepare API call parameters
function prepareApiCallUrl(mode) {
    var base = $('meta[name="api-endpoint"]').attr("content");

    var params = "?period=" + appState.period
     + "&date=" + appState[mode];

    // TEST MODE - remove when live API endpoint as added
    base = '/data/graph-' + appState.period + (mode == 'comparison' ? '-comparison' : '') +'.json'; // change this to the final API endpoint

    return base + params;
}

// Init Graph
function updateGraph() {
    $("#graph-container").css('opacity', 0.5);
    Highcharts.seriesTypes.line.prototype.getPointSpline = Highcharts.seriesTypes.spline.prototype.getPointSpline;
    Highcharts.getJSON(
        prepareApiCallUrl('date'),
        function (data) {

            $("#graph-container").css('opacity', 1);

            // Render total value
            appState.totals[0] = data.price.total;
            appState.totals[1] = data.usage.total;
            $('.graph-sum').find('.value').text(appState.units === 'eur' ? appState.totals[0] : appState.totals[1]);

            appState.eiChart = Highcharts.chart('graph-container', {
                chart: {
                    height: 300,
                    type: 'area',
                    zoomType: '',
                    backgroundColor: 'transparent',
                    animation: false
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: document.ontouchstart === undefined ?
                        '' : ''
                },

                xAxis: graphXAxisSetup(),
                yAxis: [
                    // Y Axis for Euro
                    {
                        title: { text: '' },
                        labels: { formatter: function() { return currencyFormatter.format(this.value); } },
                        min: 0,
                        gridLineColor: '#525252',
                        gridLineDashStyle: 'Dot'
                    },
                    // Y Axis for kWh
                    {
                        title: { text: '' },
                        labels: { formatter: function() { return this.value + ' kWh'; } },
                        min: 0,
                        gridLineColor: '#525252',
                        gridLineDashStyle: 'Dot'
                    }],

                // Responsive Setup
                responsive: setupGraphResponsiveRules(),

                tooltip: {
                    headerFormat: '',
                    borderWidth: 0,
                    borderColor: 'white',
                    backgroundColor: 'white',
                    borderRadius: 12,
                    shadow: true,
                    shared: true,
                    useHTML: true
                },
                legend: {
                    enabled: false
                },
                plotOptions: {
                    area: {
                        lineWidth: 2,
                        states: {
                            hover: {
                                lineWidth: 2,
                            }
                        },
                        threshold: null
                    }
                },
                states: {
                    hover: {
                        enabled: true
                    }
                },

                // Data series
                series: [{
                    name: 'Amount Euro',
                    yAxis: 0,
                    point: { events: setupTickHighlight() },
                    tooltip: { pointFormatter: euroPointFormatter },
                    visible: appState.units == 'eur',
                    data: data.price.values,
                    lineColor: '#009fda',
                    color: {
                        linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                        stops: [
                            [0, 'rgba(0, 159, 218, 0.5)'],
                            [1, Highcharts.color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                        ]
                    },
                    marker: {
                        lineWidth: 2,
                        radius: 2,
                        fillColor: '#ffffff',
                        lineColor: '#009fda',
                        symbol: 'circle',
                        enabled: false
                    }
                },{
                    name: 'Amount kWh',
                    yAxis: 1,
                    point: { events: setupTickHighlight() },
                    tooltip: { pointFormatter: kWhPointFormatter },
                    visible: appState.units == 'kwh',
                    data: data.usage.values,
                    lineColor: '#009fda',
                    color: {
                        linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                        stops: [
                            [0, 'rgba(0, 159, 218, 0.5)'],
                            [1, Highcharts.color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                        ]
                    },
                    marker: {
                        lineWidth: 2,
                        radius: 2,
                        fillColor: '#ffffff',
                        lineColor: '#009fda',
                        symbol: 'circle',
                        enabled: false
                    }
                },{
                    name: 'Amount Euro (comparison)',
                    yAxis: 0,
                    point: { events: setupTickHighlight() },
                    tooltip: { pointFormatter: euroPointComparisonFormatter },
                    data: data.price.values,
                    lineColor: '#7e7e7e',
                    visible: false,
                    color: {
                        linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                        stops: [
                            [0, 'rgba(126, 126, 126, 0.5)'],
                            [1, Highcharts.color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                        ]
                    },
                    marker: {
                        lineWidth: 2,
                        radius: 2,
                        fillColor: '#ffffff',
                        lineColor: '#7e7e7e',
                        symbol: 'circle',
                        enabled: false
                    }
                },{
                    name: 'Amount kWh (comparison)',
                    yAxis: 1,
                    point: { events: setupTickHighlight() },
                    tooltip: { pointFormatter: kWhPointComparisonFormatter },
                    visible: false,
                    data: data.usage.values,
                    lineColor: '#7e7e7e',
                    color: {
                        linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                        stops: [
                            [0, 'rgba(126, 126, 126, 0.5)'],
                            [1, Highcharts.color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                        ]
                    },
                    marker: {
                        lineWidth: 2,
                        radius: 2,
                        fillColor: '#ffffff',
                        lineColor: '#7e7e7e',
                        symbol: 'circle',
                        enabled: false
                    }
                }
                ]
            });

            // Resize graph on banner close
            $('.banner-skyscraper').on('close.bs.alert', function () {
                appState.eiChart.setSize(1100, 300);
            });

            loadComparisonData();
        }
    );

}

// Load Comparison Data
function loadComparisonData() {
    if (appState.comparison) {
        Highcharts.getJSON(
            prepareApiCallUrl('comparison'),
            function (data) {
                appState.eiChart.series[2].setVisible(appState.units === 'eur' && appState.comparison, false);
                appState.eiChart.series[3].setVisible(appState.units === 'kwh' && appState.comparison, false);
                appState.eiChart.redraw();
                appState.eiChart.series[2].setData(data.price.values, false, false, false);
                appState.eiChart.series[3].setData(data.usage.values, false, false, false);
                appState.eiChart.redraw();
                appState.totals[2] = data.price.total;
                appState.totals[3] = data.usage.total;
                if (appState.totals[0] > 0 && appState.totals[1] > 0) {
                    appState.diffs = [
                        ((appState.totals[0] / appState.totals[2] - 1) * 100).toFixed(1) + '%',
                        ((appState.totals[1] / appState.totals[3] - 1) * 100).toFixed(1) + '%'
                    ];
                }
                updateTotals();
            }
        );
    }
}

function updateTotals() {
    $('.graph-sum.comparison').find('.value').text(appState.units === 'eur' ? appState.totals[2] : appState.totals[3]);
    $('.graph-sum').find('.percentage').text(appState.units === 'eur' ? appState.diffs[0] : appState.diffs[1]);
    var negative = appState.units === 'eur' && appState.diffs[0] < 0
        || appState.units === 'kwh' && appState.diffs[1] < 0
    $('.graph-sum').find('.percentage').toggleClass('negative', negative);
}

function initDatePicker(datePicker, mode) {
    window.pickerIterator = 0;
    var period = appState.period;
    var viewMode = period + 's';
    var calendarWeeks = false;
    var format = 'YYYY-MM-DD';
    var selectedDate = moment(appState.date).format('YYYY-MM-DD');
    var minDate = window.minDate.clone();
    var maxDate = window.maxDate.clone();
    var allowMultidate = false;
    var comparisonDate = '';

    datePicker.addClass(viewMode);

    if ( viewMode === 'days') {
        if (mode === 'comparison-modal') {
            comparisonDate = moment(appState.date).subtract(1, 'days').format('YYYY-MM-DD');
        }
    }
    if ( viewMode === 'weeks') {
        viewMode = 'days';
        calendarWeeks = true;
        selectedDate = moment(appState.date).startOf('isoWeek').format('YYYY-MM-DD');
        minDate = minDate.startOf('isoWeek');
        maxDate = maxDate.startOf('isoWeek');
        if (mode === 'comparison-modal') {
            comparisonDate = moment(appState.date).subtract(1, 'weeks').startOf('isoWeek').format('YYYY-MM-DD');
        }
    }

    if ( viewMode === 'months' ) {
        format = 'MMMM';
        selectedDate = moment(appState.date).startOf('month').format('YYYY-MM-DD');
        minDate = minDate.startOf('month');
        maxDate = maxDate.startOf('month');
        if (mode === 'comparison-modal') {
            comparisonDate = moment(appState.date).subtract(1, 'months').startOf('month').format('YYYY-MM-DD');
        }
    }

    if ( viewMode === 'years' ) {
        format = 'YYYY';
        selectedDate = moment(appState.date).startOf('year').format('YYYY-MM-DD');
        minDate = minDate.startOf('year');
        maxDate = maxDate.startOf('year');
        if (mode === 'comparison-modal') {
            comparisonDate = moment(appState.date).subtract(1, 'years').startOf('year').format('YYYY-MM-DD');
        }
    }

    if (mode === 'comparison-modal') {
        datePicker.prop('mode', mode);
        datePicker.toggleClass('comparison');
        $('.selected-range-compare').addClass('active');
        $('#comparisonModal .modal-footer .date-compare').text(getFormattedDateByPeriod(appState.period, comparisonDate));
        $('#comparisonModal .modal-footer .date').text(getFormattedDateByPeriod(appState.period, selectedDate));
        allowMultidate = true;
    } else if (mode === 'comparison') {
        datePicker.prop('mode', mode);
        datePicker.addClass('comparison-single');
        selectedDate = appState.comparison;
        $('.selected-range-compare').addClass('active');
    } else {
        $('.selected-range').addClass('active');
        datePicker.removeClass('comparison-single');
    }

    datePicker.datetimepicker({
        format: format,
        inline: true,
        date: selectedDate,
        minViewMode: viewMode,
        viewMode: viewMode,
        calendarWeeks: calendarWeeks,
        minDate: minDate.format("YYYY-MM-DD"),
        maxDate: maxDate.format("YYYY-MM-DD"),
        useCurrent: false,
        allowMultidate: allowMultidate
    });

    if (mode === 'comparison-modal') datePicker.datetimepicker('setDate', [ comparisonDate, 1 ]);

    // Select calendar week
    datePicker.on('click', 'td.cw', function(ev) {
        ev.preventDefault();
        $(this).next('td').trigger('click');
    });
    // Handle click
    datePicker.on('change.datetimepicker', handleDatePickerChange);


    datePicker.toggleClass('fade show');
}

// different periods will be presented differently in the chart
function graphXAxisSetup() {
    var config = {
        labels: {
            rotation: 0,
            step: 1
        },
        crosshair: {
            color: '#005293',
            dashStyle: 'Dash',
            width: 1
        },
        lineWidth: 0,
        tickLength: 0,
        tickPositions: undefined

    };
    switch (appState.period) {
        case "day":
            config.categories = ['00h', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10',
                '11', '12', '13', '14', '15', '16', '17', '18', '19', '20',
                '21', '22', '23', '24h'];
            config.title = { text: 'Days' };
            break;
        case "week":
            config.categories = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
            config.title = { text: 'Weeks' };
            break;
        case "month":
            var daysInMonth = moment(appState.date, "YYYY-MM").daysInMonth(),
                daysInMonthArray = [];
            for(var i = 1; i < daysInMonth+1; i++) {
                daysInMonthArray.push(i);
            }
            config.tickPositions = daysInMonthArray;
            config.categories = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10',
                '11', '12', '13', '14', '15', '16', '17', '18', '19', '20',
                '21', '22', '23', '24', '25', '26', '27', '28', '29', '30',
                '31'];
            config.title = { text: 'Months' };
            break;
        case "year":
            config.categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            config.title = { text: 'Years' };
            break;
    }
    return config;
}

// Datepicker date choose
function handleDatePickerChange(ev) {
    if (!ev.date) return false;
    var datePicker = $(ev.target);
    var selectedRangeElem = $('.selected-range');
    var comparisonMode = $(ev.target).hasClass('comparison');
    var comparisonSingleMode = $(ev.target).hasClass('comparison-single');
    var selectedDate = ev.date.format('YYYY-MM-DD');
    var period = appState.period;

    if (comparisonMode) {
        selectedRangeElem = $('.selected-range-compare');
        var dates = datePicker.datetimepicker('getDates');
        if (dates.length == 3) {
            pickerIterator = (!pickerIterator) * 1;
            var newDate = dates[2].format('YYYY-MM-DD');
            datePicker.datetimepicker('setDate', [ null, 2] );
            datePicker.datetimepicker('setDate', [ newDate, pickerIterator] );
        }
    }
    if (comparisonSingleMode) {
        selectedRangeElem = $('.selected-range-compare');
    }

    selectedRangeElem.attr('data-day', moment(selectedDate, 'YYYY-MM-DD').format('ddd'));

    // Add selected date in pick date mode
    if (ev.oldDate !== null) {
        switch (period) {
            case "week":
                selectedDate = ev.date.startOf('isoWeek').format('YYYY-MM-DD');
                $('div[data-period="week"]').find(selectedRangeElem).text(moment(selectedDate, 'YYYY-MM-DD').startOf('isoWeek').format('Do') + ' - ' +
                    moment(selectedDate, 'YYYY-MM-DD').endOf('isoWeek').format('Do MMM YY'));
                break;
            case "month":
                selectedDate = ev.date.startOf('month').format('YYYY-MM-DD');
                $('div[data-period="month"]').find(selectedRangeElem).text(moment(selectedDate, 'YYYY-MM-DD').format('MMMM'));
                break;
            case "year":
                selectedDate = ev.date.startOf('year').format('YYYY-MM-DD');
                $('div[data-period="year"]').find(selectedRangeElem).text(moment(selectedDate, 'YYYY-MM-DD').format('YYYY'));
                break;
            default:
                selectedDate = ev.date.format('YYYY-MM-DD');
                $('div[data-period="day"]').find(selectedRangeElem).text(moment(selectedDate, 'YYYY-MM-DD').format('ddd Do MMM YY'));
                break;
        }

        // Handle data for pick date or comparison
        if (ev.currentTarget.id === 'eidatetimepicker-comparison') {
            var target = $('#comparisonModal').find('.modal-footer .date-compare');
            if (pickerIterator == 0) target = $('#comparisonModal').find('.modal-footer .date');
            target.text(getFormattedDateByPeriod(appState.period, selectedDate));
        } else {
            appState.period = period;
            if (comparisonMode || comparisonSingleMode) {
                appState.comparison = selectedDate;
                $('.graph-wrapper .graph-sum .title').eq(1).html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.date));
                $('.graph-wrapper .graph-sum .title').eq(0).html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.comparison));
            }
            else {
                appState.date = selectedDate;
            }
            updateUrlFromAppState();
            destroyDatePicker(datePicker);
            updateGraph();
        }
    }
}

// Setup responsive rules for graph
function setupGraphResponsiveRules() {
    var responsiveXAxisOptions =  {};
    // Different options for periods
    switch (appState.period) {
        case "day":
            responsiveXAxisOptions.labels = {
                step: 6
            };
            break;
        case "week":
            responsiveXAxisOptions.labels = {
                formatter: function () {
                    return this.value.charAt(0);
                }
            };
            break;
        case "month":
            responsiveXAxisOptions.labels = {
                step: 2
            };
            break;
        case "year":
            responsiveXAxisOptions.labels = {
                formatter: function () {
                    return this.value.charAt(0);
                }
            };
            break;
    }

    return {
        rules: [{
            condition: {
                maxWidth: 767
            },
            // Make the labels less space demanding on mobile
            chartOptions: {
                xAxis: responsiveXAxisOptions
            }
        }]
    }
}
