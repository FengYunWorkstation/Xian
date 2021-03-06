/* License
 *
 * Copyright (c) 2012, ClearCanvas Inc.
 * All rights reserved.
 * http://www.clearcanvas.ca
 *
 * This software is licensed under the Open Software License v3.0.
 * For the complete license, see http://www.clearcanvas.ca/OSLv3.0
 *
 */

/////////////////////////////////////////////////////////////////////////////////////////////////////////
/// This script contains the javascript component class for the study search panel

// Define and register the control type.
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.prototype = 
{
    initialize : function() {
                       
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.callBaseMethod(this, 'initialize');        
            
        this._OnServiceLockListRowClickedHandler = Function.createDelegate(this,this._OnServiceLockListRowClicked);
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad : function()
    {                          
        var serviceLocklist = $find(this._ServiceLockListClientID);
        serviceLocklist.add_onClientRowClick(this._OnServiceLockListRowClickedHandler);
                           
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnServiceLockListRowClicked : function(sender, event)
    {           
        this._updateToolbarButtonStates();        
    },
                       
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var serviceLocklist = $find(this._ServiceLockListClientID);               
                               
        this._enableEditButton(false);
                                           
        if (serviceLocklist!=null )
        {
            var rows = serviceLocklist.getSelectedRowElements();

            if(rows != null && rows.length > 0) {
                this._enableEditButton(true);
            }
        }
    },

    _enableEditButton : function(en)
    {
        var editButton = $find(this._EditButtonClientID);
        editButton.set_enable(en);
    },       

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_EditButtonClientID : function() {
        return this._EditButtonClientID;
    },

    set_EditButtonClientID : function(value) {
        this._EditButtonClientID = value;
        this.raisePropertyChanged('EditButtonClientID');
    },
               
    get_ServiceLockListClientID : function() {
        return this._ServiceLockListClientID;
    },

    set_ServiceLockListClientID : function(value) {
        this._ServiceLockListClientID = value;
        this.raisePropertyChanged('ServiceLockListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();