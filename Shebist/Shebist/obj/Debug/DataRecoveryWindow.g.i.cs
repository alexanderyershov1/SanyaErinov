﻿#pragma checksum "..\..\DataRecoveryWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A154BCCC17BA21DC8CE11A7E98A035218766A4C9"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Shebist;
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


namespace Shebist {
    
    
    /// <summary>
    /// DataRecoveryWindow
    /// </summary>
    public partial class DataRecoveryWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\DataRecoveryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox EnterYourEmailTextBox;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\DataRecoveryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label EnterYourEmailLabel;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\DataRecoveryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label BackLabel;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\DataRecoveryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\DataRecoveryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ConfirmGrid;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\DataRecoveryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ConfirmTextBox;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\DataRecoveryWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ConfirmLabel;
        
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
            System.Uri resourceLocater = new System.Uri("/Shebist;component/datarecoverywindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DataRecoveryWindow.xaml"
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
            this.EnterYourEmailTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.EnterYourEmailLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.BackLabel = ((System.Windows.Controls.Label)(target));
            
            #line 19 "..\..\DataRecoveryWindow.xaml"
            this.BackLabel.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.BackLabel_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 20 "..\..\DataRecoveryWindow.xaml"
            this.BackLabel.MouseEnter += new System.Windows.Input.MouseEventHandler(this.BackLabel_MouseEnter);
            
            #line default
            #line hidden
            
            #line 21 "..\..\DataRecoveryWindow.xaml"
            this.BackLabel.MouseLeave += new System.Windows.Input.MouseEventHandler(this.BackLabel_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 4:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\DataRecoveryWindow.xaml"
            this.OkButton.Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ConfirmGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.ConfirmTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 39 "..\..\DataRecoveryWindow.xaml"
            this.ConfirmTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ConfirmTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ConfirmLabel = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

