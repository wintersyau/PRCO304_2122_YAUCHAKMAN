#pragma checksum "..\..\..\OneAction.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "31F67D1ECEBC3A8B76D572C47DDA4DAFE4DBF3729D167CC18545A234E31D9B2E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WinDefense;


namespace WinDefense {
    
    
    /// <summary>
    /// OneAction
    /// </summary>
    public partial class OneAction : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 33 "..\..\..\OneAction.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ProcessName;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\OneAction.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ProcessPath;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\OneAction.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ProcessAccess;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\OneAction.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox Signs;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\OneAction.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CurrentSelectStr;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WinDefense;component/oneaction.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\OneAction.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ProcessName = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.ProcessPath = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            
            #line 44 "..\..\..\OneAction.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenPath);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ProcessAccess = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.Signs = ((System.Windows.Controls.ListBox)(target));
            
            #line 57 "..\..\..\OneAction.xaml"
            this.Signs.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Signs_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.CurrentSelectStr = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            
            #line 70 "..\..\..\OneAction.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.PassProcess);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 80 "..\..\..\OneAction.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrustProcess);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 90 "..\..\..\OneAction.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.KillProcess);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

