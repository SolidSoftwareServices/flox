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
        "period": "year",
        "date": maxDate.startOf('isoWeek').format('YYYY-MM-DD'),
        "units": "eur",
        "eiChart": null,
        "totals": [0, 0]
    };

    initAppStateFromUrl();
    updateGraph();

    // Switch Euro / kWh
    $('.graph-value-switch input').on('change', function(ev) {
        var total = 0;
        if (this.id === 'value-eur') {
            appState.eiChart.series[0].show();
            appState.eiChart.series[1].hide();
            appState.units = 'eur';
        } else if ( this.id === 'value-kwh' ) {
            appState.eiChart.series[0].hide();
            appState.eiChart.series[1].show();
            appState.units = 'kwh';
            total = 1;
        }
        $('.graph-sum').find('.amount').attr('data', this.id).find('.value').text(appState.totals[total]);
        updateUrlFromAppState();
    });

    // Change year
    $('.period-nav button').on('click', function(ev) {
        ev.preventDefault();
        var period = appState.period;
        var direction = $(this).data('mode');
        var currentRange = $(this).closest('.period-nav').find('.selected-range').text();
        var filterElem = $('#pills-' + period + ' .period-nav .selected-range');
        switch (period) {
            case "year":
                switchYear(currentRange, direction, filterElem);
                break;
            default:
                break;
        }
    });

});


// FUNCTIONS -----------------------------------------------------------------------------------------------

// Prepare API call parameters
function prepareApiCallUrl(mode) {
    var base = $('meta[name="api-endpoint"]').attr("content");

    var params = "?period=" + appState.period
     + "&date=" + appState[mode];

    // TEST MODE - remove when live API endpoint as added
    base = '/data/graph-gas.json'; // change this to the final API endpoint

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
                    tooltip: { pointFormatter: euroPointFormatterGas },
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
                    name: 'Amount KWh',
                    yAxis: 1,
                    point: { events: setupTickHighlight() },
                    tooltip: { pointFormatter: kWhPointFormatterGas },
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
                }
                ]
            });

            // Resize graph on banner close
            $('.banner-skyscraper').on('close.bs.alert', function () {
                appState.eiChart.setSize(1100, 300);
            });
        }
    );

}

function initDatePicker(datePicker, mode) {
    var period = appState.period;
    var viewMode = period + 's';
    var calendarWeeks = false;
    var format = 'YYYY-MM-DD';
    var disabledDates = [];
    var defaultDate = moment(appState.date).format('YYYY-MM-DD');
    var minDate = window.minDate.clone();
    var maxDate = window.maxDate.clone();

    datePicker.addClass(viewMode);

    if ( viewMode === 'years' ) {
        format = 'YYYY';
        defaultDate = moment(appState.date).startOf('year').format('YYYY-MM-DD');
        minDate = moment(minDate).startOf('year').format('YYYY-MM-DD');
        maxDate = moment(maxDate).startOf('year').format('YYYY-MM-DD');
        if (mode === 'comparison') {
            disabledDates.push(moment(defaultDate, 'YYYY-MM-DD'));
            var yearToCompare = moment(appState.date).subtract(1, 'years').startOf('year').format('YYYY-MM-DD');
            defaultDate = appState.comparison ? appState.comparison : yearToCompare;
        }
    }

    datePicker.datetimepicker({
        format: format,
        inline: true,
        defaultDate: defaultDate,
        minViewMode: viewMode,
        viewMode: viewMode,
        calendarWeeks: calendarWeeks,
        minDate: minDate,
        maxDate: maxDate,
        disabledDates: disabledDates
    })

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
            rotation: 0
        },
        crosshair: {
            color: '#005293',
            dashStyle: 'Dash',
            width: 1
        },
        lineWidth: 0,
        tickLength: 0,
        title: { text: 'Two Month Periods' }

    };
    config.categories = ['Jan - Feb', 'Mar - Apr', 'May - Jun', 'Jul - Aug', 'Sep - Oct', 'Nov - Dec'];
    return config;
}

// Datepicker date choose
function handleDatePickerChange(ev) {
    var selectedRangeElem = $('.selected-range');
    var comparisonDate = $(ev.target).prop('mode') == 'comparison';
    var selectedDate = ev.date.format('YYYY-MM-DD');
    var period = appState.period;

    if (comparisonDate) {
        selectedRangeElem = $('.selected-range-compare');
    }

    // Add selected date in pick date mode
    if (ev.oldDate !== null) {

        selectedDate = ev.date.startOf('year').format('YYYY-MM-DD');
        $('div[data-period="year"]').find(selectedRangeElem).text(moment(selectedDate, 'YYYY-MM-DD').format('YYYY'));

        appState.period = period;
        appState.date = selectedDate;
        updateUrlFromAppState();
        destroyDatePicker($(ev.target));
        updateGraph();
    }
}

function updateTotals() {
    // no action needed in the gas view
}

// Setup responsive rules for graph
function setupGraphResponsiveRules() {
    var responsiveXAxisOptions =  {};
    // Different options for periods
    switch (appState.period) {
        case "year":
            responsiveXAxisOptions.labels = {
                formatter: function () {
                    return this.value.replace('-', '<br/>');
                }
            };
            break;
    }

    return {
        rules: [{
            condition: {
                maxWidth: 767
            },

            chartOptions: {
                xAxis: responsiveXAxisOptions
            }
        }]
    }
}

// Tooltip formatter
function euroPointFormatterGas() {
    var html = '',
        monthRanges = ['Jan - Feb', 'Mar - Apr', 'May - Jun', 'Jul - Aug', 'Sep - Oct', 'Nov - Dec'],
        xValue = monthRanges[this.x] + ' ' + moment(appState.date, 'YYYY-MM-DD').format('YYYY');
    if(appState.comparison) {
        html = '<div class="chart-tooltip date euro order-1"><p class="x-value">' + xValue + '</p><p class="y-value">' +
            '<span class="currency">' + currencyFormatter.format(this.y).charAt(0) + '</span>' + currencyFormatter.format(this.y).substr(1, this.length) + '</p></div>';
    } else {
        html = '<div class="chart-tooltip euro order-1"><p class="x-value">' + xValue + '</p><p class="y-value">' +
            '<span class="currency">' + currencyFormatter.format(this.y).charAt(0) + '</span>' + currencyFormatter.format(this.y).substr(1, this.length) + '</p></div>';
    }
    return html;
}

function kWhPointFormatterGas() {
    var html = '',
        monthRanges = ['Jan - Feb', 'Mar - Apr', 'May - Jun', 'Jul - Aug', 'Sep - Oct', 'Nov - Dec'],
        xValue = monthRanges[this.x] + ' ' + moment(appState.date, 'YYYY-MM-DD').format('YYYY');
    html = '<div class="chart-tooltip kwh order-1"><p class="x-value">' + xValue + '</p><p class="y-value">' +
        this.y + '<span class="unit"> kWh</p></div>';
    return html;
}
