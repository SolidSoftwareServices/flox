jQuery(document).ready(function () {
	var svg_div = jQuery('#svg_target');

	svg_div.html("");
	var data = "[[DOT_GRAPH]]";
	var svg = Viz(data, "svg");
	svg_div.html("<hr>" + svg);

});