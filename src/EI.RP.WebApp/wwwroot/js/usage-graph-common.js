$(function() {

    // Hide Datepicker when clicked outside, but not in-modal Datepicker
    $(document).on('mouseup touchend', function (e) {
        var container = $('.bootstrap-datetimepicker-widget');
        if (!container.is(e.target) && container.has(e.target).length === 0 && !container.parent().hasClass('in-modal')) {
            destroyDatePicker(container.parent());
        }
    });

    // Browser history event (Back/Forward buttons)
    $(window).on("popstate", function(ev) {
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
    var period = getParameterByName("period");
    if (period) appState.period = period;
    var date = getParameterByName("date");
    if (date && !moment(date).isAfter(maxDate)) appState.date = date;
    var units = getParameterByName("units");
    if (units) appState.units = units;
    var comparison = getParameterByName("comparison");
    if (comparison && !moment(comparison).isAfter(maxDate)) appState.comparison = comparison;

    $('.period-switcher #pills-tab li a[data-period=' + appState.period + ']').tab('show');

    $('#pills-day .period-nav .selected-range').attr('data-day', moment(appState.date).format('ddd')).text(moment(appState.date).format('ddd Do MMM YY'));
    $('#pills-week .period-nav .selected-range').text(moment(appState.date).startOf('isoWeek').format('Do') + ' - ' +  moment(appState.date).endOf('isoWeek').format('Do MMM YY'));
    $('#pills-month .period-nav .selected-range').text(moment(appState.date).format('MMMM'));
    $('#pills-year .period-nav .selected-range').text(moment(appState.date).format('YYYY'));

    $('.ei-switch').find('.comparison-period').text(appState.period + 's');

    $('.graph-value-switch input#value-eur').prop('checked', appState.units === 'eur');
    $('.graph-value-switch input#value-kwh').prop('checked', appState.units === 'kwh');
    $('.graph-sum .amount').attr('data', 'value-' + appState.units);
    updateTotals();

    if (comparison) {
        $("main.usage").addClass("comparison");
        $('.graph-wrapper .graph-sum .title').eq(1).html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.date));
        $('.graph-wrapper .graph-sum .title').eq(0).html('<span class="bullet"></span> ' + getFormattedDateByPeriod(appState.period, appState.comparison));

        $('#pills-day .period-nav .selected-range-compare').attr('data-day', moment(appState.comparison).format('ddd')).text(moment(appState.comparison).format('ddd Do MMM YY'));
        $('#pills-week .period-nav .selected-range-compare').text(moment(appState.comparison).startOf('isoWeek').format('Do') + ' - ' +  moment(appState.comparison).endOf('isoWeek').format('Do MMM YY'));
        $('#pills-month .period-nav .selected-range-compare').text(moment(appState.comparison).format('MMMM'));
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

    $(".ei-switch").find('input').prop('checked', !!appState.comparison).change();

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
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function destroyDatePicker(datePicker) {
    $('.selected-range-compare').removeClass('active');
    $('.selected-range').removeClass('active');
    datePicker.removeClass('fade show days weeks months years comparison').datetimepicker('destroy');
    datePicker.off();
}

function switchDay(currentRange, direction, elem) {
    var newRange = '',
        newRangeDay = '';
    switch(direction) {
        case "prev":
            newRange = moment(currentRange, 'ddd Do MMM YY').subtract(1, 'days').format('ddd Do MMM YY');
            newRangeDay = moment(currentRange, 'ddd Do MMM YY').subtract(1, 'days').format('ddd');
            break;
        case "next":
            newRange = moment(currentRange, 'ddd Do MMM YY').add(1, 'days').format('ddd Do MMM YY');
            newRangeDay = moment(currentRange, 'ddd Do MMM YY').add(1, 'days').format('ddd');
            break;
    }
    elem.text( newRange );
    elem.attr('data-day', newRangeDay);
    var date = moment(newRange, 'ddd Do MMM YY').format('YYYY-MM-DD');
    appState.date = date;
    appState.period = 'day';
    updateUrlFromAppState();
    updateGraph();
}

function switchWeek(currentRange, direction, elem) {
    var newRange = '';
    switch(direction) {
        case "prev":
            newRange = moment(currentRange, 'Do - Do MMM YY').subtract(1, 'weeks').startOf('isoWeek').format('Do')
                + ' - ' + moment(currentRange, 'Do - Do MMM YY').subtract(1, 'weeks').endOf('isoWeek').format('Do MMM YY');
            break;
        case "next":
            newRange = moment(currentRange, 'Do - Do MMM YY').add(1, 'weeks').startOf('isoWeek').format('Do')
                + ' - ' + moment(currentRange, 'Do - Do MMM YY').add(1, 'weeks').endOf('isoWeek').format('Do MMM YY') ;
            break;
    }
    elem.text( newRange );
    var date = moment(newRange, 'Do - Do MMM YY').startOf('isoWeek').format('YYYY-MM-DD');
    appState.date = date;
    appState.period = 'week';
    updateUrlFromAppState();
    updateGraph();
}

function switchMonth(currentRange, direction, elem) {
    var newRange = '';
    switch(direction) {
        case "prev":
            newRange = moment(appState.date, 'YYYY-MM-DD').subtract(1, 'months');
            break;
        case "next":
            newRange = moment(appState.date, 'YYYY-MM-DD').add(1, 'months');
            break;
    }
    elem.text( newRange.format('MMMM') );
    var date = moment(newRange).startOf('month').format('YYYY-MM-DD');
    appState.date = date;
    appState.period = 'month';
    updateUrlFromAppState();
    updateGraph();
}

function switchYear(currentRange, direction, elem) {
    var newRange = '';
    switch(direction) {
        case "prev":
            newRange = moment(currentRange, 'YYYY').subtract(1, 'years').format('YYYY');
            break;
        case "next":
            newRange = moment(currentRange, 'YYYY').add(1, 'years').format('YYYY');
            break;
    }
    elem.text( newRange );
    var date = moment(newRange, 'YYYY').startOf('year').format('YYYY-MM-DD');
    appState.date = date;
    appState.period = 'year';
    updateUrlFromAppState();
    updateGraph();
}

function covertDateToPeriod(date, period) {
    switch (period) {
        case "day":
            date = moment(date).format('YYYY-MM-DD');
            break;
        case "week":
            date = moment(date).startOf('isoWeek').format('YYYY-MM-DD');
            break;
        case "month":
            date = moment(date).startOf('month').format('YYYY-MM-DD');
            break;
        case "year":
            date = moment(date).startOf('year').format('YYYY-MM-DD');
            break;
    }
    return date;
}

function getFormattedDateByPeriod(period, date) {
    switch(period) {
        case "day":
            date = moment(date, 'YYYY-MM-DD').format('ddd Do MMM YY');
            break;
        case "week":
            date = moment(date, 'YYYY-MM-DD').startOf('isoWeek').format('Do') + ' - ' +
                    moment(date, 'YYYY-MM-DD').endOf('isoWeek').format('Do MMM YY');
            break;
        case "month":
            date = moment(date, 'YYYY-MM-DD').format('MMMM YYYY');
            break;
        case "year":
            date = moment(date, 'YYYY-MM-DD').format('YYYY');
            break;
    }
    return date;
}

function getFormattedDateByPeriodForTooltip(period, baseDate, point) {
    var date = '',
        comparison = getParameterByName("comparison");
    switch(period) {
        case "day":
            var time = point.x;
            if (time < 10) date = "0" + time;
            time = time + ":00";
            if (comparison) {
                date = moment(baseDate, 'YYYY-MM-DD').format('Do MMM') + ' - ' + time;
            } else {
                date = time;
            }
            break;
        case "week":
        case "month":
            date = moment(baseDate).add(point.x, 'days').format('ddd Do MMM YY');
            break;
        case "year":
            date = moment(baseDate).add(point.x, 'month').format('MMMM YYYY');
            break;
    }
    return date;
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
        case "day":
            return 3600 * 1000;
        case "week":
        case "month":
        case "year":
            break;
    }
    return 1;
}

// update page URL via JS history API
function updateUrlFromAppState() {
    var url = "?period=" + appState.period + '&date=' + appState.date + '&units=' + appState.units;
    if (appState.comparison) url += '&comparison=' + appState.comparison;
    history.pushState({
        period: appState.period,
        date: appState.date,
        comparison: appState.comparison,
        units: appState.units
    }, "Period", url);

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
    if (appState.date && moment(appState.date, 'YYYY-MM-DD').isSameOrAfter(maxDate.format('YYYY-MM-DD'), appState.period)) {
        $('.btn-next').hide();
    } else {
        $('.btn-next').show();
    }

    // Check for minDate reached
    if (appState.date && moment(appState.date, 'YYYY-MM-DD').isSameOrBefore(minDate.format('YYYY-MM-DD'), appState.period)) {
        $('.btn-prev').hide();
    } else {
        $('.btn-prev').show();
    }
}
