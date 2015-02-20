<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DescriptorEditor.ascx.cs"
	Inherits="WorkflowDesigner.ExternalDialogs.DescriptorEditor" %>
<script type="text/javascript" src="../js/common.js"></script>
<script type="text/javascript" src="../js/wfweb.js"></script>
<script type="text/javascript" src="../js/wfdesigner.js"></script>
<script type="text/javascript">
	function onClickEditor(sender, e) {
		var activeEditor = sender.get_activeEditor();
		var opType;

		if (activeEditor.get_property().editorParams) {
			var editorParam = activeEditor.get_currentEditorParams();
			opType = editorParam;
			if (typeof (editorParam) == "object") {
				if (editorParam.hasOwnProperty("tagName")) {
					opType = editorParam.tagName;
				}
			}
		} else {
			opType = activeEditor.get_property().name;
		}

		if (WFWeb.PropertGridOpenEditor[opType](activeEditor)) {
			WFWeb.PropertGridOpenEditor[opType]();
		}
	}

	function loadProcessInstanceDescription() {
		if ($get("instanceDescription") != null) {
			var descJson = $get("instanceDescription").value;

			if (descJson != '') {
				var processes = Sys.Serialization.JavaScriptSerializer.deserialize(descJson);

				for (var i = 0; i < processes.length; i++) {
					var processKey = processes[i].Key;

					if (WFWeb.GlobalProcList.Get(processKey) == undefined) {
						WFWeb.GlobalProcList.Add(processes[i]);
					}
					else {
						alert("流程模板 " + processKey + " 已经在编辑中!");
					}

					WFWeb.Property.CurrentProcessKey = processKey;

					if (typeof (afterProcessDeserialized) == "function")
						afterProcessDeserialized(processes[i]);
				}
			}
		}
	}

	function findActivityByKey(process, key) {
		var result = null;

		if (process) {
			for (var i = 0; i < process.Activities.length; i++) {
				if (process.Activities[i].Key == key) {
					result = process.Activities[i];
					break;
				}
			}
		}

		return result;
	}

	function findTransitionByKey(process, key) {
		var result = null;

		if (process) {
			for (var i = 0; i < process.Transitions.length; i++) {
				if (process.Transitions[i].Key == key) {
					result = process.Transitions[i];
					break;
				}
			}
		}

		return result;
	}
</script>
