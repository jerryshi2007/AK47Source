(function () {
	if (window.WFDesigner == undefined) window.WFDesigner = {};

	WFDesigner.DesignerInterAction = {
		SLManager: function () {
			var result = null;
			var slp = document.getElementById("SLP");

			if (slp != null)
				result = slp.Content.SLM;

			return result;
		},

		CreateNewWorkflow: function (key) {
			WFDesigner.DesignerInterAction.SLManager().CreateNewWorkflow(key);
		},

		OpenWorkflow: function (procJsonStr) {
			WFDesigner.DesignerInterAction.SLManager().OpenWorkflow(procJsonStr);
		},

		LayoutCurrentDiagram: function () {
			WFDesigner.DesignerInterAction.SLManager().LayoutCurrentDiagram();
		},

		RemoveActivityTemplate: function () {
			return WFDesigner.DesignerInterAction.SLManager().RemoveActivityTemplate();
		},

		UpdateDiagramData: function (sender, data) {
			var obj = WFWeb.Property.CurrentObj;
			var slm = WFDesigner.DesignerInterAction.SLManager();

			if (slm != null) {
				//修改的是一个流程
				if (obj.Activities != undefined) {
					slm.UpdateDiagramData("Workflow", obj.Key, data.property.name, data.property.value);
					return;
				}

				//修改的是一个活动点
				if (obj.ActivityType != undefined) {
					slm.UpdateDiagramData("Activity", obj.Key, data.property.name, data.property.value);
					return;
				}

				//修改的是一个连接线
				if (obj.Priority != undefined) {
					var name = data.property.name;
					var val = data.property.value;
					if (data.property.name == 'Name' && val == '') {
						if (obj.Condition != undefined && obj.Condition.Expression != undefined) {
							name = 'Condition';
							if (obj.Condition.Expression != '') {
								val = '[' + obj.Condition.Expression + ']';
							}
						}
					}
					slm.UpdateDiagramData("Transition", obj.Key, name, val);
				}
			}
		},

		GetWorkflowGraph: function (key) {
			return WFDesigner.DesignerInterAction.SLManager().GetWorkflowGraph(key);
		},
		AddActivitySelfLink: function () {
			WFDesigner.DesignerInterAction.SLManager().AddActivitySelfLink();
		}
	};


}
)();