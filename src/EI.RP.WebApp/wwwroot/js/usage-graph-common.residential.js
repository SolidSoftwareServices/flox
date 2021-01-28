
// Highcharts responsive option update workaround
Highcharts.addEvent(
    Highcharts.Chart,
    'update',
    function(event) {
        var chart = this;
        if (event.options && event.options.responsive) {
            this.userOptions.responsive = this.options.responsive = event.options.responsive;
        }
    }
);

$(function() {

    // Hide Datepicker when clicked outside, but not in-modal Datepicker
    $(document).on('mouseup touchend', function (e) {
        var container = $('.bootstrap-datetimepicker-widget');
        if (!container.is(e.target) && container.has(e.target).length === 0 && !container.parent().hasClass('in-modal')) {
            destroyDatePicker(container.parent());
        }
    });

    // Browser history event (Back/Forward buttons)
    $(window).on('popstate', function(ev) {
        var prevPeriod = window.appState.period;
        var prevDate = window.appState.date;
        var prevComparison = window.appState.comparison;
        var prevUnits = window.appState.units;
        initAppStateFromUrl();
        var newPeriod = window.appState.period;
        var newDate = window.appState.date;
        var newComparison = window.appState.comparison;
        var newUnits = window.appState.units;
        if (newPeriod != prevPeriod ||
            newDate != prevDate ||
            newComparison != prevComparison ||
            newUnits != prevUnits) {
            updateGraph();
        }
    });

});

// Currency formatter helper function
var currencyFormatter = { format: function(value) {
    return '€' + value.toFixed(2);
}};

// Init app state object from URL
function initAppStateFromUrl() {
    var i = getParameterByName('i');
    if (i) appState.i = i;
    var period = getParameterByName('period');
    if (period) appState.period = period;
    var date = getParameterByName('date');
    if (date) {
        var d = moment();
        if (moment(date).isAfter(appState.maxDate)) {
            d = appState.maxDate;
        }
        else if (moment(date).isBefore(appState.minDate)) {
            d = appState.minDate;
        } else {
            d = moment(date);
        }
        appState.date = d.format('YYYY-MM-DD');
    }
    var units = getParameterByName('units');
    if (units) appState.units = units;
    var comparison = getParameterByName('comparison');
    if (comparison && !moment(comparison).isAfter(appState.maxDate)) appState.comparison = comparison;

    $('.period-switcher #pills-tab a[data-period=' + (appState.period === 'bimonthly' ? 'year' : appState.period) + ']').tab('show');

    $('#pills-day .period-nav .selected-range').attr('data-day', moment(appState.date).format('ddd')).text(moment(appState.date).format('ddd Do MMM YY'));
    $('#pills-week .period-nav .selected-range').text(moment(appState.date).startOf('isoWeek').format('Do') + ' - ' +  moment(appState.date).endOf('isoWeek').format('Do MMM YY'));
    $('#pills-month .period-nav .selected-range').text(moment(appState.date).format('MMMM YYYY'));
    $('#pills-year .period-nav .selected-range').text(moment(appState.date).format('YYYY'));

    $('.ei-switch').find('.comparison-period').text(appState.period === 'bimonthly' ? 'Years' : appState.period + 's');

    $('.graph-value-switch input#value-eur').prop('checked', appState.units === 'eur');
    $('.graph-value-switch input#value-kwh').prop('checked', appState.units === 'kwh');
    $('.graph-sum .amount').attr('data', 'value-' + appState.units);
    updateTotals();

    if (comparison) {
        $('main.usage').addClass('comparison');
        $('.graph-wrapper .graph-sum .title').eq(1).html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.date));
        $('.graph-wrapper .graph-sum .title').eq(0).html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.comparison));

        $('#pills-day .period-nav .selected-range-compare').attr('data-day', moment(appState.comparison).format('ddd')).text(moment(appState.comparison).format('ddd Do MMM YY'));
        $('#pills-week .period-nav .selected-range-compare').text(moment(appState.comparison).startOf('isoWeek').format('Do') + ' - ' +  moment(appState.comparison).endOf('isoWeek').format('Do MMM YY'));
        $('#pills-month .period-nav .selected-range-compare').text(moment(appState.comparison).format('MMMM YYYY'));
        $('#pills-year .period-nav .selected-range-compare').text(moment(appState.comparison).format('YYYY'));

    } else {
        $('.graph-wrapper .graph-sum .title').html('Total');
    }

    if (appState.eiChart) {
        appState.eiChart.series[0].setVisible(appState.units === 'eur', false);
        appState.eiChart.series[1].setVisible(appState.units === 'kwh', false);
        appState.eiChart.series[2].setVisible(appState.units === 'eur' && appState.comparison, false);
        appState.eiChart.series[3].setVisible(appState.units === 'kwh' && appState.comparison, false);
        appState.eiChart.redraw();
    }

    $('.ei-switch').find('input').prop('checked', !!appState.comparison).change();

    checkMinMaxDate();
}

// Get parameter from URL
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return name !== 'i' 
        ? decodeURIComponent(results[2].replace(/\+/g, ' ')) 
        : results[2];
}

function destroyDatePicker(datePicker) {
    $('.selected-range-compare').removeClass('active');
    $('.selected-range').removeClass('active');
    datePicker.removeClass('fade show days weeks months years comparison').datetimepicker('destroy');
    datePicker.off();
}

function switchDay(currentRange, direction, elem) {
    var newRange = moment(currentRange, 'ddd Do MMM YY');

        switch(direction) {
        case 'prev':
            newRange = newRange.subtract(1, 'days');
            break;
        case 'next':
            newRange = newRange.add(1, 'days');
            break;
    }

    if (newRange.isBefore(appState.minDate)) {
        newRange = moment(appState.minDate);
    } else if (newRange.isAfter(appState.maxDate)) {
        newRange = moment(appState.maxDate);
    }

    elem.text(newRange.format('ddd Do MMM YY'));
    elem.attr('data-day', newRange.format('ddd'));
    appState.date = newRange.format('YYYY-MM-DD');
    appState.period = 'day';
    updateUrlFromAppState();
    updateGraph();
}

function switchWeek(currentRange, direction, elem) {
    var startOfWeek = moment(currentRange, 'Do - Do MMM YY'),
        endOfWeek = moment(currentRange, 'Do - Do MMM YY');

    switch(direction) {
        case 'prev':
            startOfWeek = startOfWeek.subtract(1, 'weeks');
            endOfWeek = endOfWeek.subtract(1, 'weeks');
            break;
        case 'next':
            startOfWeek = startOfWeek.add(1, 'weeks');
            endOfWeek = endOfWeek.add(1, 'weeks');
            break;
    }

    startOfWeek = startOfWeek.startOf('isoWeek');
    endOfWeek = endOfWeek.endOf('isoWeek');

    if (startOfWeek.isBefore(appState.minDate)) {
        startOfWeek = appState.minDate;
    } else if (endOfWeek.isAfter(appState.maxDate)) {
        endOfWeek = appState.maxDate;
    }

    elem.text(startOfWeek.format('Do') 
        + ' - ' + endOfWeek.format('Do MMM YY'));
    appState.date = startOfWeek.format('YYYY-MM-DD');
    appState.period = 'week';
    updateUrlFromAppState();
    updateGraph();
}

function switchMonth(currentRange, direction, elem) {
    var newRange = moment(appState.date, 'YYYY-MM-DD');

    switch(direction) {
        case 'prev':
            newRange = newRange.subtract(1, 'months');
            break;
        case 'next':
            newRange = newRange.add(1, 'months');
            break;
    }

    newRange = newRange.startOf('month');

    if (newRange.isBefore(appState.minDate)) {
        newRange = appState.minDate;
    } else if (newRange.isAfter(appState.maxDate)) {
        newRange = appState.maxDate;
    }

    elem.text(newRange.format('MMMM'));
    appState.date = newRange.format('YYYY-MM-DD');
    appState.period = 'month';
    updateUrlFromAppState();
    updateGraph();
}

function switchBimonthly(currentRange, direction, elem) {
    var newRange = moment(currentRange, 'YYYY');

    switch(direction) {
        case 'prev':
            newRange = newRange.subtract(1, 'years');
            break;
        case 'next':
            newRange = newRange.add(1, 'years');
            break;
    }

    newRange = newRange.startOf('year');

    if (newRange.isBefore(appState.minDate)) {
        newRange = appState.minDate;
    } else if (newRange.isAfter(appState.maxDate)) {
        newRange = appState.maxDate;
    }

    elem.text(newRange.format('YYYY'));
    appState.date = newRange.format('YYYY-MM-DD');
    appState.period = 'bimonthly';
    updateUrlFromAppState();
    updateGraph();
}

function switchYear(currentRange, direction, elem) {
    var newRange = moment(currentRange, 'YYYY');

    switch(direction) {
        case 'prev':
            newRange = newRange.subtract(1, 'years');
            break;
        case 'next':
            newRange = newRange.add(1, 'years');
            break;
    }

    newRange = newRange.startOf('year');

    if (newRange.isBefore(appState.minDate)) {
        newRange = appState.minDate;
    } else if (newRange.isAfter(appState.maxDate)) {
        newRange = appState.maxDate;
    }

    elem.text(newRange.format('YYYY'));
    appState.date = newRange.format('YYYY-MM-DD');
    appState.period = 'year';
    updateUrlFromAppState();
    updateGraph();
}

function covertDateToPeriod(date, period) {
    var d = dateToPeriodStart(date, period);

    if (d.isBefore(appState.minDate)) {
        d = appState.minDate;
    } else if (d.isAfter(appState.maxDate)) {
        d = appState.maxDate;
    }

    return d.format('YYYY-MM-DD');
}

function dateToPeriodStart(date, period) {
    switch (period) {
        case 'day':
            return moment(date);
        case 'week':
            return moment(date).startOf('isoWeek');
        case 'month':
            return moment(date).startOf('month');
        case 'bimonthly':
        case 'year':
            return moment(date).startOf('year');
    }
}

function getFormattedDateByPeriod(period, date, returnYearOnlyForBimonthly) {
    switch(period) {
        case 'day':
            date = moment(date, 'YYYY-MM-DD').format('ddd Do MMM YY');
            break;
        case 'week':
            date = moment(date, 'YYYY-MM-DD').startOf('isoWeek').format('Do') + ' - ' +
                    moment(date, 'YYYY-MM-DD').endOf('isoWeek').format('Do MMM YY');
            break;
        case 'month':
            date = moment(date, 'YYYY-MM-DD').startOf('month').format('MMMM YYYY');
            break;
        case 'bimonthly':
            date = returnYearOnlyForBimonthly 
                ? moment(date, 'YYYY-MM-DD').format('YYYY') 
                : getFormattedDateForBimonthlyData(date);
            break;
        case 'year':
            if (appState.hasNonSmartData) {
                date = returnYearOnlyForBimonthly 
                    ? moment(date, 'YYYY-MM-DD').format('YYYY') 
                    : getFormattedDateForBimonthlyData(date);
                break;
            }
            date = moment(date, 'YYYY-MM-DD').format('YYYY');
            break;
    }
    return date;
}

function getFormattedDateForBimonthlyData(date) {
    var monthRanges = {
        1: 'Jan - Feb',
        2: 'Jan - Feb',
        3: 'Mar - Apr',
        4: 'Mar - Apr',
        5: 'May - Jun',
        6: 'May - Jun',
        7: 'Jul - Aug',
        8: 'Jul - Aug',
        9: 'Sep - Oct',
        10: 'Sep - Oct',
        11: 'Nov - Dec',
        12: 'Nov - Dec'
    };
    return monthRanges[moment(date, 'YYYY-MM-DD').format('M')] + ' ' + moment(date, 'YYYY-MM-DD').format('YYYY');
}

function getFormattedDateByPeriodForTooltip(period, baseDate, point) {
    var date = '',
        comparison = getParameterByName('comparison');
    switch(period) {
        case 'day':
            var time = point.x;
            if (time < 10) date = '0' + time;
            time = time + ':00';
            if (comparison) {
                date = moment(baseDate, 'YYYY-MM-DD').format('Do MMM') + ' - ' + time;
            } else {
                date = time;
            }
            break;
        case 'week':
            date = moment(baseDate).startOf('isoWeek').add(point.x, 'days').format('ddd Do MMM YY');
            break;
        case 'month':
            date = moment(baseDate).startOf('month').add(point.x, 'days').format('ddd Do MMM YY');
            break;
        case 'bimonthly':
            date = getFormattedDateByPeriodForBimonthlyDataTooltip(baseDate, point);
            break;
        case 'year':
            if (appState.hasNonSmartData) {
                date = getFormattedDateByPeriodForBimonthlyDataTooltip(baseDate, point);
                break;
            }
            date = moment(baseDate).startOf('year').add(point.x, 'month').format('MMMM YYYY');
            break;
    }
    return date;
}

function getFormattedDateByPeriodForBimonthlyDataTooltip(date, point) {
    var monthRanges = ['Jan - Feb', 'Mar - Apr', 'May - Jun', 'Jul - Aug', 'Sep - Oct', 'Nov - Dec'];
    return monthRanges[point.x] + ' ' + moment(date, 'YYYY-MM-DD').format('YYYY');
}

function euroPointFormatter() {
    var html = '',
        xValue = getFormattedDateByPeriodForTooltip(appState.period, appState.date, this);
    if(appState.comparison) {
        html = '<div class="chart-tooltip date euro order-1"><p class="x-value">' + xValue + '</p><p class="y-value">' +
            '<span class="currency">' + currencyFormatter.format(this.y).charAt(0) + '</span>' + currencyFormatter.format(this.y).substr(1, this.length) + '</p></div>';
    } else {
        html = '<div class="chart-tooltip euro order-1"><p class="x-value">' + xValue + '</p><p class="y-value">' +
            '<span class="currency">' + currencyFormatter.format(this.y).charAt(0) + '</span>' + currencyFormatter.format(this.y).substr(1, this.length) + '</p></div>';
    }
    return html;
}

function euroPointComparisonFormatter() {
    var html = '',
        xValue = getFormattedDateByPeriodForTooltip(appState.period, appState.comparison, this);
    html = '<div class="chart-tooltip comparison euro order-0"><p class="x-value">' + xValue + '</p><p class="y-value">' +
           '<span class="currency">' + currencyFormatter.format(this.y).charAt(0) + '</span>' + currencyFormatter.format(this.y).substr(1, this.length) + '</p></div>';
    return html;
}

function kWhPointFormatter() {
    var html = '';
    var xValue = getFormattedDateByPeriodForTooltip(appState.period, appState.date, this);
    html = '<div class="chart-tooltip kwh order-1"><p class="x-value">' + xValue + '</p><p class="y-value">' +
           this.y + '<span class="unit"> kWh</p></div>';
    return html;
}

function kWhPointComparisonFormatter() {
    var html = '';
    var xValue = getFormattedDateByPeriodForTooltip(appState.period, appState.comparison, this);
    html = '<div class="chart-tooltip comparison kwh order-0"><p class="x-value">' + xValue + '</p><p class="y-value">' +
           this.y + '<span class="unit"> kWh</p></div>';
    return html;
}

function getPointInterval() {
   switch (appState.period) {
        case 'day':
            return 3600 * 1000;
        case 'week':
        case 'month':
        case 'bimonthly':
        case 'year':
            break;
    }
    return 1;
}

// update page URL via JS history API
function updateUrlFromAppState() {
    var url = '?i=' + appState.i
        + '&period=' + appState.period 
        + '&date=' + appState.date 
        + '&units=' + appState.units;
    if (appState.comparison) url += '&comparison=' + appState.comparison;
    history.pushState({
        i: appState.i,
        period: appState.period,
        date: appState.date,
        comparison: appState.comparison,
        units: appState.units
    }, 'Period', url);

    checkMinMaxDate();
}

// highlight label on y-axis on hover
function setupTickHighlight() {
    return {
        mouseOver: function(ev) {
            this.selectedTick = this.series.xAxis.ticks[this.x];
            if (this.selectedTick && this.selectedTick.label) {
                this.selectedTick.label.css({
                    color:'#525252',
                    fontSize: '14px',
                    fontWeight: 'bold'
                });
            }
        },
        mouseOut: function(e) {
            if (this.selectedTick && this.selectedTick.label) {
                this.selectedTick.label.css({
                    color: '#525252',
                    fontWeight: 'normal',
                    fontSize: '11px'
                });
                this.selectedTick = null;
            }
        }
    }
}

// Check Min / Max date and hide prev next if limits reached
function checkMinMaxDate() {
    // Check for maxDate reached
    if (appState.date && 
        moment(appState.date, 'YYYY-MM-DD').isSameOrAfter(
            appState.maxDate.format('YYYY-MM-DD'), 
            (appState.period === 'bimonthly' ? 'year' : appState.period))) {
        $('.btn-next').hide();
    } else {
        $('.btn-next').show();
    }

    // Check for minDate reached
    if (appState.date && 
        moment(appState.date, 'YYYY-MM-DD').isSameOrBefore(
            appState.minDate.format('YYYY-MM-DD'), 
            (appState.period === 'bimonthly' ? 'year' : appState.period))) {
        $('.btn-prev').hide();
    } else {
        $('.btn-prev').show();
    }
}

function KeepDateInBoundaries(date) {
    if (date.isBefore(appState.minDate)) {
        date = appState.minDate;
    } else if (date.isAfter(appState.maxDate)) {
        date = appState.maxDate;
    }
    return date;
}