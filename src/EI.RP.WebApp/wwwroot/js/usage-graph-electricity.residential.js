$(function () {

    // Init Moment.js
    moment.updateLocale('en', {
        week: {
            dow: 1 // Week starts on Mondays
        }
    });

    var $chartContainer = $('[data-chart]');

    // Min + Max Date - backend service should provide this
    var minDate = moment($chartContainer.data('chart-min-date'));
    var maxDate = moment($chartContainer.data('chart-max-date'));

    var isSmart = $chartContainer.data('chart-is-smart') === true;
    var hasSmartData = $chartContainer.data('chart-has-smart-data') === true;
    var hasNonSmartData = $chartContainer.data('chart-has-non-smart-data') === true;
    var startDatesSmartPlan = $chartContainer.data('chart-start-dates-smart')
        .split(',')
        .map(function (d) {
            return moment(d, 'YYYY-MM-DD');
        });
    var endDatesSmartPlan = $chartContainer.data('chart-end-dates-smart')
        .split(',')
        .map(function (d) {
            return moment(d, 'YYYY-MM-DD');
        });
    var startDatesNonSmartPlan = $chartContainer.data('chart-start-dates-non-smart')
        .split(',')
        .map(function (d) {
            return moment(d, 'YYYY-MM-DD');
        });
    var showPlanUpdatePlotLines = $chartContainer.data('chart-show-plan-update-plot-lines') === true;

    var period = 'month';
    if (!isSmart) {
        period = 'bimonthly';
    }
 


    // App State Object (defaults)
    window.appState = {
        'i': null,
        'minDate': minDate,
        'maxDate': maxDate,
        'period': period,
        'date': maxDate.clone().startOf('isoWeek').format('YYYY-MM-DD'),
        'comparison': null,
        'units': 'eur',
        'eiChart': null,
        'totals': [0, 0, 0, 0],
        'diffs': [0, 0],
        'isSmart': isSmart,
        'hasSmartData': hasSmartData,
        'hasNonSmartData': hasNonSmartData,
        'startDatesSmartPlan': startDatesSmartPlan,
        'endDatesSmartPlan': endDatesSmartPlan,
        'startDatesNonSmartPlan': startDatesNonSmartPlan,
        'showPlanUpdatePlotLines': showPlanUpdatePlotLines
    };

    initAppStateFromUrl();
    initGraph();
    updateGraph();

    // Period Switcher
    $('.period-switcher a.nav-link').on('click', function (ev) {
        ev.preventDefault();
        appState.period = $(this).data('period');
        appState.date = covertDateToPeriod(appState.date, appState.period);
        appState.comparison = null;
        $('.ei-switch').find('.comparison-period').text(appState.period === 'bimonthly' ? 'Years' : appState.period + 's');
        updateUrlFromAppState();
        initAppStateFromUrl();
        updateGraph();
    });

    // Switch Euro / kWh
    $('.graph-value-switch input').on('change', function (ev) {
        var total = 0;
        if (this.id === 'value-eur') {
            appState.units = 'eur';
        } else if (this.id === 'value-kwh') {
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
    $('.period-nav button').on('click', function (ev) {
        ev.preventDefault();
        var period = appState.period;
        var direction = $(this).data('mode');
        var currentRange = $(this).closest('.period-nav').find('.selected-range').text();
        var filterElem = $('#pills-' + (period === 'bimonthly' ? 'year' : period) + ' .period-nav .selected-range');
        switch (period) {
            case 'day':
                switchDay(currentRange, direction, filterElem);
                break;
            case 'week':
                switchWeek(currentRange, direction, filterElem);
                break;
            case 'month':
                switchMonth(currentRange, direction, filterElem);
                break;
            case 'bimonthly':
                switchBimonthly(currentRange, direction, filterElem);
                break;
            case 'year':
                switchYear(currentRange, direction, filterElem);
                break;
            default:
                break;
        }
    });

    // Comparison modal
    $('#compareSwitch').on('change', function () {
        if ($(this).prop('checked')) {
            $('#comparisonModal').modal('toggle');
        } else {
            appState.comparison = null;
            $('.graph-wrapper .graph-sum .title').html('Total');
            updateUrlFromAppState();
            if ($('main.usage').hasClass('comparison')) {
                $('main.usage').removeClass('comparison');
                updateGraph();
            }
        }
    });

    $('#comparisonModal').on('show.bs.modal', function (ev) {
        $(this).find('.modal-title .comparison-period').text(appState.period === 'bimonthly' ? 'Years' : appState.period + 's');
        initDatePicker($('#eidatetimepicker-comparison'), 'comparison-modal');
    }).on('hidden.bs.modal', function (ev) {
        destroyDatePicker($('#eidatetimepicker-comparison'));
        if (!appState.comparison) $('#compareSwitch').prop('checked', false).change();
    }).on('click', '#compareNow', function (ev) {
        ev.stopPropagation();
        var dates = $('#eidatetimepicker-comparison').datetimepicker('getDates');

        var date = moment(dates[0]);
        if (date.isBefore(appState.minDate)) {
            date = appState.minDate;
        } else if (date.isAfter(appState.maxDate)) {
            date = appState.maxDate;
        }
        appState.date = date.format('YYYY-MM-DD');

        var comparisonDate = moment(dates[1]);
        if (comparisonDate.isBefore(appState.minDate)) {
            comparisonDate = appState.minDate;
        } else if (comparisonDate.isAfter(appState.maxDate)) {
            comparisonDate = appState.maxDate;
        }
        appState.comparison = comparisonDate.format('YYYY-MM-DD');

        updateUrlFromAppState();
        initAppStateFromUrl();
        loadComparisonData();
    });

    // Show DatePicker when date is clicked
    $('.selected-range').on('click', function (ev) {
        ev.preventDefault();
        initDatePicker($('#eidatetimepicker'), undefined);
    });

    // Show DatePicker when comprison date is clicked
    $('.selected-range-compare').on('click', function (ev) {
        ev.preventDefault();
        initDatePicker($('#eidatetimepicker'), 'comparison');
    });

    // Listen to DOM mutations
    $(document).on('DOMNodeInserted', function (ev) {
        // Add .active / .disabled class to table row "week" period (not possible with pure CSS)
        $('.ei-datepicker').find('td.day.active').parent().addClass('active');
        $('.ei-datepicker').find('td.day.index-1').parent().addClass('index-1');
    });

});


// FUNCTIONS -----------------------------------------------------------------------------------------------

// Prepare API call parameters
function prepareApiCallUrl(mode) {
    var base = $('[data-chart]').data('chart-endpoint-url');
    var accountNumber = $('[data-chart]').data('chart-account-number');

    var params = '?period=' + appState.period
        + '&date=' + appState[mode]
        + '&accountNumber=' + accountNumber;

   
    return base + params;
}

// Init Graph
function initGraph() {
    Highcharts.seriesTypes.line.prototype.getPointSpline = Highcharts.seriesTypes.spline.prototype.getPointSpline;
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
        yAxis: graphYAxisSetup(),
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
        credits: {
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
            name: 'graphDataEur',
            yAxis: 0,
            point: { events: setupTickHighlight() },
            tooltip: { pointFormatter: euroPointFormatter },
            visible: appState.units == 'eur',
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
        }, {
            name: 'graphDataKwh',
            yAxis: 1,
            point: { events: setupTickHighlight() },
            tooltip: { pointFormatter: kWhPointFormatter },
            visible: appState.units == 'kwh',
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
        }, {
            name: 'graphDataEurComparison',
            yAxis: 0,
            point: { events: setupTickHighlight() },
            tooltip: { pointFormatter: euroPointComparisonFormatter },
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
        }, {
            name: 'graphDataKwhComparison',
            yAxis: 1,
            point: { events: setupTickHighlight() },
            tooltip: { pointFormatter: kWhPointComparisonFormatter },
            visible: false,
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
}

// Update Graph
function updateGraph() {

    if (appState.eiChart) {
        appState.eiChart.showLoading();
    }

    Highcharts.ajax(
        {
            url: prepareApiCallUrl('date'),
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                if (appState.eiChart) {
                    appState.eiChart.hideLoading();
                }

                // Render total value
                appState.totals[0] = data.price.total;
                appState.totals[1] = data.usage.total;
                $('.graph-sum').find('.value').text(appState.units === 'eur' ? appState.totals[0].toFixed(2) : appState.totals[1]);

                appState.eiChart.series[0].setData(data.price.values);
                appState.eiChart.series[1].setData(data.usage.values);
                appState.eiChart.series[2].setData(data.price.values);
                appState.eiChart.series[3].setData(data.usage.values);

                var minPrice = Math.min.apply(null, data.price.values);
                var minUsage = Math.min.apply(null, data.usage.values);

                appState.eiChart.update({
                    responsive: setupGraphResponsiveRules(),
                    xAxis: graphXAxisSetup(),
                    yAxis: graphYAxisSetup(minPrice, minUsage)
                });

                // Resize graph on banner close
                $('.banner-skyscraper').on('close.bs.alert', function () {
                    appState.eiChart.setSize(1100, 300);
                });

                loadComparisonData();
            },
            error: function () {
                if (appState.eiChart) {
                    appState.eiChart.hideLoading();
                }
            }
        }
    );

}

// Load Comparison Data
function loadComparisonData() {
    if (appState.comparison) {

        if (appState.eiChart) {
            appState.eiChart.showLoading();
        }

        Highcharts.ajax(
            {
                url: prepareApiCallUrl('comparison'),
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    if (appState.eiChart) {
                        appState.eiChart.hideLoading();
                    }

                    appState.eiChart.series[2].setVisible(appState.units === 'eur' && appState.comparison, false);
                    appState.eiChart.series[3].setVisible(appState.units === 'kwh' && appState.comparison, false);
                    appState.eiChart.redraw();

                    appState.eiChart.series[2].setData(data.price.values, false, false, false);
                    appState.eiChart.series[3].setData(data.usage.values, false, false, false);
                    appState.eiChart.redraw();

                    appState.totals[2] = data.price.total;
                    appState.totals[3] = data.usage.total;

                    var zeroesCheck =
                        [
                            appState.totals[0] + appState.totals[2] === 0,
                            appState.totals[1] + appState.totals[3] === 0
                        ];

                    appState.diffs = [
                        zeroesCheck[0] ? 0 : (((appState.totals[0] / appState.totals[2]) * 100) - 100).toFixed(1),
                        zeroesCheck[1] ? 0 : (((appState.totals[1] / appState.totals[3]) * 100) - 100).toFixed(1)
                    ];

                    updateTotals();
                },
                error: function () {
                    if (appState.eiChart) {
                        appState.eiChart.hideLoading();
                    }
                }
            }
        );

    }
}



function updateTotals() {
    if (appState.comparison == null) {
        $('.graph-sum .title')
            .eq(1)
            .html('Total');
    }
    else {
        $('.graph-sum.comparison .title')
            .eq(0)
            .html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.comparison, true));
        $('.graph-sum .title')
            .eq(1)
            .html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.date, true));
    }

    $('.graph-sum.comparison')
        .find('.value')
        .text(appState.units === 'eur'
            ? appState.totals[2].toFixed(2)
            : appState.totals[3]);

    $('.graph-sum')
        .find('.percentage')
        .text(appState.units === 'eur'
            ? getIncDecPercentage(appState.diffs[0])
            : getIncDecPercentage(appState.diffs[1]));

    var negative = appState.units === 'eur' && appState.diffs[0] < 0
        || appState.units === 'kwh' && appState.diffs[1] < 0;
    $('.graph-sum')
        .find('.percentage')
        .toggleClass('negative', negative);
}

function getIncDecPercentage(val) {
    return val == Infinity ? '100%' : val + '%';
}

function initDatePicker(datePicker, mode) {
    window.pickerIterator = 0;
    var period = appState.period;
    var viewMode = period + 's';
    var calendarWeeks = false;
    var format = 'YYYY-MM-DD';
    var minDate = appState.minDate.clone();
    var maxDate = appState.maxDate.clone();
    var selectedDate = moment(appState.date);
    var allowMultidate = false;
    var comparisonDate = moment();

    datePicker.addClass(viewMode);

    if (viewMode === 'days') {
        if (mode === 'comparison-modal') {
            comparisonDate = KeepDateInBoundaries(
                moment(appState.date).subtract(1, 'days'));
        }
    }

    if (viewMode === 'weeks') {
        viewMode = 'days';
        calendarWeeks = true;
        selectedDate = KeepDateInBoundaries(
            moment(appState.date).startOf('isoWeek'));
        if (mode === 'comparison-modal') {
            comparisonDate = KeepDateInBoundaries(
                moment(appState.date).subtract(1, 'weeks').startOf('isoWeek'));
        }
    }

    if (viewMode === 'months') {
        format = 'MMMM';
        selectedDate = KeepDateInBoundaries(
            moment(appState.date).startOf('month'));
        minDate = minDate.startOf('month');
        maxDate = maxDate.endOf('month');
        if (mode === 'comparison-modal') {
            comparisonDate = KeepDateInBoundaries(
                moment(appState.date).subtract(1, 'months').startOf('month'));
        }
    }

    if (viewMode === 'bimonthlys' || viewMode === 'years') {
        viewMode = 'years';
        format = 'YYYY';
        selectedDate = KeepDateInBoundaries(
            moment(appState.date).startOf('year'));
        minDate = minDate.startOf('year');
        maxDate = maxDate.endOf('year');
        if (mode === 'comparison-modal') {
            comparisonDate = KeepDateInBoundaries(
                moment(appState.date).subtract(1, 'years').startOf('year'));
        }
    }

    if (mode === 'comparison-modal') {
        datePicker.prop('mode', mode);
        datePicker.toggleClass('comparison');
        $('.selected-range-compare').addClass('active');
        $('#comparisonModal .modal-footer .date-compare').text(getFormattedDateByPeriod(appState.period, comparisonDate, true));
        $('#comparisonModal .modal-footer .date').text(getFormattedDateByPeriod(appState.period, selectedDate, true));
        allowMultidate = true;
    } else if (mode === 'comparison') {
        datePicker.prop('mode', mode);
        datePicker.addClass('comparison-single');
        selectedDate = KeepDateInBoundaries(
            moment(appState.comparison));
        $('.selected-range-compare').addClass('active');
    } else {
        $('.selected-range').addClass('active');
        datePicker.removeClass('comparison-single');
    }

    datePicker.datetimepicker({
        format: format,
        inline: true,
        date: selectedDate.format('YYYY-MM-DD'),
        minViewMode: viewMode,
        viewMode: viewMode,
        calendarWeeks: calendarWeeks,
        minDate: minDate.format('YYYY-MM-DD'),
        maxDate: maxDate.format('YYYY-MM-DD'),
        useCurrent: false,
        allowMultidate: allowMultidate,
    });

    if (mode === 'comparison-modal') {
        datePicker.datetimepicker('setDate', [comparisonDate.format('YYYY-MM-DD'), 1]);
        if (viewMode === 'days' || viewMode === 'weeks') {
            datePicker.datetimepicker('disabledDates', false);
        } else {
            UpdateDisabledDates(datePicker, appState.date);
        }
    } else if (mode === 'comparison') {
        UpdateDisabledDates(datePicker, appState.date);
    } else {
        UpdateDisabledDates(datePicker, appState.comparison);
    }

    // Select calendar week
    datePicker.on('click', 'td.cw', function (ev) {
        ev.preventDefault();
        $(this).next('td').trigger('click');
    });
    // Handle click
    datePicker.on('change.datetimepicker', handleDatePickerChange);

    datePicker.toggleClass('fade show');
}

function UpdateDisabledDates(datePicker, date) {
    datePicker.datetimepicker('disabledDates', ResolveDateRangeByPeriod(date, appState.period));
}

function ResolveDateRangeByPeriod(date, period) {
    switch (period) {
        case 'day':
            return [date];
        case 'week':
            return GetDaysInWeek(date);
        case 'month':
            return GetDaysInMonth(date);
        case 'bimonthly':
        case 'year':
            return GetDaysInYear(date);
    }
}

function GetDaysInWeek(date) {
    var d = dateToPeriodStart(date, 'week');
    var week = d.week();
    var result = [];
    while (d.week() === week) {
        result.push(d.clone());
        d.add(1, 'd');
    }
    return result;
}

function GetDaysInMonth(date) {
    var d = dateToPeriodStart(date, 'month');
    var month = d.month();
    var result = [];
    while (d.month() === month) {
        result.push(d.clone());
        d.add(1, 'd');
    }
    return result;
}

function GetDaysInYear(date) {
    var d = dateToPeriodStart(date, 'year');
    var year = d.year();
    var result = [];
    while (d.year() === year) {
        result.push(d.clone());
        d.add(1, 'd');
    }
    return result;
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
        startOnTick: true,
        plotLines: []
    };
    switch (appState.period) {
        case 'day':
            config.categories = ['00h', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10',
                '11', '12', '13', '14', '15', '16', '17', '18', '19', '20',
                '21', '22', '23', '24h'];
            break;
        case 'week':
            config.categories = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
            config.plotLines = getPlotLines('week');
            break;
        case 'month':
            var daysInMonth = moment(appState.date, 'YYYY-MM').daysInMonth(),
                daysInMonthArray = [];
            for (var i = 1; i < daysInMonth + 1; i++) {
                daysInMonthArray.push(i);
            }
            config.categories = daysInMonthArray;
            config.plotLines = getPlotLines('month');
            break;
        case 'bimonthly':
            config.categories = getCategoriesForBimonthlyData();
            config.plotLines = getPlotLines('bimonthly');
            break;
        case 'year':
            if (appState.hasNonSmartData) {
                config.categories = getCategoriesForBimonthlyData();
                config.plotLines = getPlotLines('bimonthly');
                break;
            }
            config.categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            config.plotLines = getPlotLines('year');
            break;
    }
    return config;
}

function getCategoriesForBimonthlyData() {
    return ['Jan - Feb', 'Mar - Apr', 'May - Jun', 'Jul - Aug', 'Sep - Oct', 'Nov - Dec'];
}

function getPlotLines(period) {
    var plotLines = [];

    if (!appState.showPlanUpdatePlotLines ||
        (appState.isSmart && !appState.hasNonSmartData) ||
        (!appState.isSmart && !appState.hasSmartData)) {
        return plotLines;
    }

    var currentDate = moment(appState.date, 'YYYY-MM-DD');

    appState.startDatesSmartPlan.forEach(function (date) {
        if (currentDate.isSame(date, period === 'bimonthly' ? 'year' : period)) {
            if (currentDate.isSame(appState.minDate, 'day')) {
                return;
            }
            plotLines.push(
                getPlotLineObject(resolvePlotLineValue(period, date), 'Smart Usage')
            );
        }
    });

    appState.startDatesNonSmartPlan.forEach(function (date) {
        if (currentDate.isSame(date, period === 'bimonthly' ? 'year' : period)) {
            if (currentDate.isSame(appState.minDate, 'day')) {
                return;
            }
            plotLines.push(
                getPlotLineObject(resolvePlotLineValue(period, date), 'End of Smart Usage')
            );
        }
    });

    return plotLines;
}

function resolvePlotLineValue(period, date) {
    switch (period) {
        case 'week':
            return date.day();
        case 'month':
            return date.date();
        case 'bimonthly':
            var monthRanges = { 0: 0, 1: 0, 2: 1, 3: 1, 4: 2, 5: 2, 6: 3, 7: 3, 8: 4, 9: 4, 10: 5, 11: 5 };
            return monthRanges[date.month()];
        case 'year':
            return date.month();
        default:
            return '';
    }
}

function getPlotLineObject(value, label) {
    return {
        color: '#098900',
        dashStyle: 'Dash',
        width: 1,
        value: value,
        label: {
            text: label
        }
    }
}

function graphYAxisSetup(minPrice, minUsage) {

    if (!minPrice || minPrice > 0) {
        minPrice = 0;
    }

    if (!minUsage || minUsage > 0) {
        minUsage = 0;
    }

    return [
        // Y Axis for Euro
        {
            title: { text: '' },
            labels: { formatter: function () { return currencyFormatter.format(this.value); } },
            min: minPrice,
            gridLineColor: '#525252',
            gridLineDashStyle: 'Dot'
        },
        // Y Axis for kWh
        {
            title: { text: '' },
            labels: { formatter: function () { return this.value + ' kWh'; } },
            min: minUsage,
            gridLineColor: '#525252',
            gridLineDashStyle: 'Dot'
        }
    ];
}

// Datepicker date choose
function handleDatePickerChange(ev) {
    if (!ev.date) return false;
    var datePicker = $(ev.target);
    var selectedRangeElem = $('.selected-range');
    var comparisonMode = $(ev.target).hasClass('comparison');
    var comparisonSingleMode = $(ev.target).hasClass('comparison-single');
    var selectedDate = ev.date;
    var period = appState.period;

    if (comparisonMode) {
        selectedRangeElem = $('.selected-range-compare');
        var dates = datePicker.datetimepicker('getDates');
        if (dates.length == 3) {
            pickerIterator = (!pickerIterator) * 1;
            var newDate = dates[2].format('YYYY-MM-DD');
            datePicker.datetimepicker('setDate', [null, 2]);
            datePicker.datetimepicker('setDate', [newDate, pickerIterator]);
        }
    }
    if (comparisonSingleMode) {
        selectedRangeElem = $('.selected-range-compare');
    }

    selectedRangeElem.attr('data-day', moment(selectedDate, 'YYYY-MM-DD').format('ddd'));

    // Add selected date in pick date mode
    if (ev.oldDate !== null) {
        switch (period) {
            case 'week':
                var startOfWeek = moment(selectedDate).startOf('isoWeek'),
                    endOfWeek = moment(selectedDate).endOf('isoWeek');
                if (startOfWeek.isBefore(appState.minDate)) {
                    startOfWeek = appState.minDate;
                } else if (endOfWeek.isAfter(appState.maxDate)) {
                    endOfWeek = appState.maxDate;
                }
                selectedDate = startOfWeek;
                $('div[data-period="week"]').find(selectedRangeElem).text(startOfWeek.format('Do') + ' - ' +
                    endOfWeek.format('Do MMM YY'));
                break;
            case 'month':
                selectedDate = moment(selectedDate).startOf('month');
                if (selectedDate.isBefore(appState.minDate)) {
                    selectedDate = appState.minDate;
                } else if (selectedDate.isAfter(appState.maxDate)) {
                    selectedDate = appState.maxDate;
                }
                $('div[data-period="month"]').find(selectedRangeElem).text(selectedDate.format('MMMM YYYY'));
                break;
            case 'bimonthly':
            case 'year':
                selectedDate = moment(selectedDate).startOf('year');
                if (selectedDate.isBefore(appState.minDate)) {
                    selectedDate = appState.minDate;
                } else if (selectedDate.isAfter(appState.maxDate)) {
                    selectedDate = appState.maxDate;
                }
                $('div[data-period="year"]').find(selectedRangeElem).text(selectedDate.format('YYYY'));
                break;
            default:
                selectedDate = moment(selectedDate);
                if (selectedDate.isBefore(appState.minDate)) {
                    selectedDate = moment(appState.minDate);
                } else if (selectedDate.isAfter(appState.maxDate)) {
                    selectedDate = moment(appState.maxDate);
                }
                $('div[data-period="day"]').find(selectedRangeElem).text(selectedDate.format('ddd Do MMM YY'));
                break;
        }

        // Handle data for pick date or comparison
        if (ev.currentTarget.id === 'eidatetimepicker-comparison') {
            var target = $('#comparisonModal').find('.modal-footer .date-compare');
            if (appState.period === "day" || appState.period === "week") {
                if (pickerIterator === 0) target = $('#comparisonModal').find('.modal-footer .date');
            } else {
                if (pickerIterator !== 0) target = $('#comparisonModal').find('.modal-footer .date');
            }
            target.text(getFormattedDateByPeriod(appState.period, selectedDate.format('YYYY-MM-DD'), true));
        } else {
            appState.period = period;
            if (comparisonMode || comparisonSingleMode) {
                appState.comparison = selectedDate.format('YYYY-MM-DD');
                $('.graph-wrapper .graph-sum .title').html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.date, true));
                $('.graph-wrapper .graph-sum.comparison .title').html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.comparison, true));
            }
            else {
                appState.date = selectedDate.format('YYYY-MM-DD');
            }
            updateUrlFromAppState();
            destroyDatePicker(datePicker);
            updateGraph();
        }
    }
}

// Setup responsive rules for graph
function setupGraphResponsiveRules() {
    var responsiveXAxisOptions = {};
    // Different options for periods
    switch (appState.period) {
        case 'day':
            responsiveXAxisOptions.labels = {
                step: 6
            };
            break;
        case 'week':
            responsiveXAxisOptions.labels = {
                formatter: function () {
                    return this.value && this.value.length > 1
                        ? this.value.charAt(0)
                        : '';
                }
            };
            break;
        case 'month':
            responsiveXAxisOptions.labels = {
                step: 2
            };
            break;
        case 'bimonthly':
            responsiveXAxisOptions.labels = setupResponsiveXAxisLabelsForBimonthlyData();
            break;
        case 'year':
            if (appState.hasNonSmartData) {
                responsiveXAxisOptions.labels = setupResponsiveXAxisLabelsForBimonthlyData();
                break;
            }
            responsiveXAxisOptions.labels = {
                formatter: function () {
                    return this.value && this.value.length > 1
                        ? this.value.charAt(0)
                        : '';
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

function setupResponsiveXAxisLabelsForBimonthlyData() {
    return {
        formatter: function () {
            return this.value && this.value.length > 1
                ? this.value.replace('-', '<br/>')
                : '';
        }
    };
}