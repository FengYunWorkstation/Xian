/////////////////////////////////////////////////////////////////////////////////////////////////////////
///
/// This script contains the javascript component class for the study search panel
/// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////

// Define and register the control type.
//
// Only define and register the type if it doens't exist. Otherwise "... does not derive from Sys.Component" error 
// will show up if multiple instance of the controls must be created. The error is misleading. It looks like the type 
// is RE-define for the 2nd instance but registerClass() will fail so the type will be essential undefined when the object
// is instantiated.
//

if (window.__registeredTypes['ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel']==null)
{

    Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive');

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructor
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel = function(element) { 
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.initializeBase(this, [element]);
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Create the prototype for the control.
    //
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
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
        //
        // Events
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        _OnLoad : function()
        {                    
            var PartitionArchivelist = $find(this._PartitionArchiveListClientID);
            PartitionArchivelist.add_onClientRowClick(this._OnPartitionArchiveListRowClickedHandler);
                 
            this._updateToolbarButtonStates();
        },
        
        // called when user clicked on a row in the study list
        _OnPartitionArchiveListRowClicked : function(sender, event)
        {    
            this._updateToolbarButtonStates();        
        },
                       
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Private Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        _updateToolbarButtonStates : function()
        {
            var PartitionArchivelist = $find(this._PartitionArchiveListClientID);
                                         
            this._enableEditButton(false);
            this._enableDeleteButton(false);
                                           
            if (PartitionArchivelist!=null )
            {
                var rows = PartitionArchivelist.getSelectedRowElements();

                if(rows != null && rows.length > 0) {
                    this._enableEditButton(true);
                    this._enableDeleteButton(true);
                }
            }
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
        //
        // Public methods
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Properties
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
               
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
    }

    // Register the class as a type that inherits from Sys.UI.Control.
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel', Sys.UI.Control);
     

    if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

}