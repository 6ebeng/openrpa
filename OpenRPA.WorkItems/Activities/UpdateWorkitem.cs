﻿using System;
using System.Activities;
using OpenRPA.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRPA.Interfaces.Input;
using OpenRPA.Interfaces.entity;
using System.Data;

namespace OpenRPA.WorkItems
{
    [System.ComponentModel.Designer(typeof(UpdateWorkitemDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [System.Drawing.ToolboxBitmap(typeof(UpdateWorkitem), "Resources.toolbox.updateworkitem.png")]
    [LocalizedToolboxTooltip("activity_updateworkitem_tooltip", typeof(Resources.strings))]
    [LocalizedDisplayName("activity_updateworkitem", typeof(Resources.strings))]
    public class UpdateWorkitem : AsyncTaskCodeActivity
    {
        [LocalizedDisplayName("activity_updateworkitem_workitem", typeof(Resources.strings)), LocalizedDescription("activity_updateworkitem_workitem_help", typeof(Resources.strings))]
        public InArgument<IWorkitem> Workitem { get; set; }
        [LocalizedDisplayName("activity_updateworkitem_state", typeof(Resources.strings)), LocalizedDescription("activity_updateworkitem_state_help", typeof(Resources.strings))]
        public InArgument<string> State { get; set; }
        [LocalizedDisplayName("activity_updateworkitem_files", typeof(Resources.strings)), LocalizedDescription("activity_updateworkitem_files_help", typeof(Resources.strings))]
        public InArgument<string[]> Files { get; set; }
        protected async override Task<object> ExecuteAsync(AsyncCodeActivityContext context)
        {
            var status = new string[] { "failed", "successful", "abandoned", "retry", "processing" };
            var files = Files.Get<string[]>(context);
            var t = Workitem.Get(context);
            if (t == null) throw new Exception("Missing Workitem");
            if(State != null && State.Expression != null)
            {
                var state = State.Get(context);
                if (!string.IsNullOrEmpty(state)) t.state = state;
            }
            t.state = t.state.ToLower();
            if (!status.Contains(t.state)) throw new Exception("Illegal state on Workitem, must be failed, successful, abandoned or retry");
            var result = await global.webSocketClient.UpdateWorkitem<Workitem>(t, files);
            return result;
        }
        protected override void AfterExecute(AsyncCodeActivityContext context, object result)
        {
        }
        [LocalizedDisplayName("activity_displayname", typeof(Resources.strings)), LocalizedDescription("activity_displayname_help", typeof(Resources.strings))]
        public new string DisplayName
        {
            get
            {
                var displayName = base.DisplayName;
                if (displayName == this.GetType().Name)
                {
                    var displayNameAttribute = this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
                    if (displayNameAttribute != null) displayName = displayNameAttribute.DisplayName;
                }
                return displayName;
            }
            set
            {
                base.DisplayName = value;
            }
        }

    }

}