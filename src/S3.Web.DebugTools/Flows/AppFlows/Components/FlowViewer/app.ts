import G = require("./../../../../scripts/graphs/ggraph");
import IDDSVGGraph = require("./../../../../scripts/graphs/iddsvggraph");

declare var JPPicker;
var graphView = document.getElementById("graphView");
var graphControl = new IDDSVGGraph(graphView);
graphControl.allowEditing = false;
var graph: G.GGraph = null; 



function generateGraph() {
	var jsonHidden = <HTMLInputElement>document.getElementById("flow_debugger_graph");
	var jsonText = jsonHidden.value;
	graph = G.GGraph.ofJSON(jsonText);
	graphControl.setGraph(graph); 
	graph.settings.transformation = G.GPlaneTransformation.ninetyDegreesTransformation;
	
	graph.createNodeBoundariesForSVGInContainer(graphView);
	graph.layoutCallbacks.add(() => {
		graphControl.drawGraph();
	});
	graph.beginLayoutGraph();

	
}
function generateStepsData() {
	const $pathTarget = document.querySelectorAll('.path');
	const $source = document.querySelector('#json-renderer');

	const defaultOpts = {
		processKeys: false,
		outputCollapsed: true

	};
	
		let jsonData = null;
		jsonData = (<HTMLInputElement>	document.getElementById("flow_steps_data")).value;
		jsonData = JSON.parse(jsonData);

		JPPicker.render($source, jsonData, $pathTarget, defaultOpts);
}


generateGraph();
generateStepsData();
