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
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.prototype = 
{
    initialize : function() {
       
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.callBaseMethod(this, 'initialize');        
            
        this._OnPartitionArchiveListRowClickedHandler = Function.createDelegate(this,this._OnPartitionArchiveListRowClicked);
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad : function()
    {                    
        var PartitionArchiveList = $find(this._PartitionArchiveListClientID);
        PartitionArchiveList.add_onClientRowClick(this._OnPartitionArchiveListRowClickedHandler);
                 
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnPartitionArchiveListRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
                       
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var PartitionArchiveList = $find(this._PartitionArchiveListClientID);
                                         
        this._enableEditButton(false);
        this._enableDeleteButton(false);
                                                      
        if (PartitionArchiveList!=null)
        {
            var rows = PartitionArchiveList.getSelectedRowElements();

            if (rows!=null && rows.length>0)
            {
		        var selectedArchiveCount = rows.length; 
		        var canDeleteCount=0; 
				for(i=0; i<rows.length; i++)
                {
                    if (this._canDeleteArchive(rows[i]))
                    {
                        canDeleteCount++;
                    }
                }
                    
                this._enableDeleteButton(canDeleteCount>0);
                    
                // always enabled open button when a row is selected
                this._enableEditButton(true);
            }
        }
    },
        
    _canDeleteArchive : function(row)
    {
        //"candelete" is a custom attribute injected by the list control
        return row.getAttribute('candelete')=='true';
    },
        
    _enableDeleteButton : function(en)
    {
        var deleteButton = $find(this._DeleteButtonClientID);
        deleteButton.set_enable(en);
    },
        
    _enableEditButton : function(en)
    {
        var editButton = $find(this._EditButtonClientID);
        editButton.set_enable(en);
    },       

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_DeleteButtonClientID : function() {
        return this._DeleteButtonClientID;
    },

    set_DeleteButtonClientID : function(value) {
        this._DeleteButtonClientID = value;
        this.raisePropertyChanged('DeleteButtonClientID');
    },
        
    get_EditButtonClientID : function() {
        return this._EditButtonClientID;
    },

    set_EditButtonClientID : function(value) {
        this._EditButtonClientID = value;
        this.raisePropertyChanged('EditButtonClientID');
    },
               
    get_PartitionArchiveListClientID : function() {
        return this._PartitionArchiveListClientID;
    },

    set_PartitionArchiveListClientID : function(value) {
        this._PartitionArchiveListClientID = value;
        this.raisePropertyChanged('PartitionArchiveListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
