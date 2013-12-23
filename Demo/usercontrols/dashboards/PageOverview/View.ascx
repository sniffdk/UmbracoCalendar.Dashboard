<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Demo.usercontrols.dashboards.PageOverview.View" %>
<%@ Register TagPrefix="CD" Namespace="ClientDependency.Core.Controls" Assembly="ClientDependency.Core" %>
<CD:CssInclude runat="server" FilePath="/userControls/dashboards/PageOverview/fuelux.css" />
<CD:JsInclude runat="server" FilePath="/userControls/dashboards/PageOverview/underscore.min.js" />
<CD:JsInclude runat="server" FilePath="/userControls/dashboards/PageOverview/fuelux.loader.js" />
<CD:JsInclude runat="server" FilePath="/userControls/dashboards/PageOverview/fuelux.datagrid.datasource.js" />
<!-- Just a few custom styles I'm going to need, nothing fancy .. -->
<style>
    .fuelux .datagrid { margin: 10px 0 0 0; }
    .fuelux .datagrid-header-right > .select,
    .fuelux .datagrid-header-right > .input-append { float: left; }
    .fuelux .datagrid tbody tr.odd { background-color: #f9f9f9; }
</style>
<div class="container fuelux">
    <table id="DashboardDatagrid" class="table table-bordered datagrid">
        <thead>
            <tr>
                <th>
                    <span class="datagrid-header-title">Page overview</span>
                    <div class="datagrid-header-right">
                        <div class="select filter" data-resize="">
                            <button type="button" data-toggle="dropdown" class="btn dropdown-toggle">
                                <span class="dropdown-label"></span>
                                <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu">
                                <li data-value="mypages" data-selected="true"><a href="#">My pages only</a></li>
						        <li data-value="all"><a href="#">All pages</a></li>
                            </ul>
                        </div>
                        <div class="input-append search datagrid-search">
                            <input type="text" class="input-medium" placeholder="Search">
                            <button type="button" class="btn"><i class="icon-search"></i>&nbsp;</button>
                        </div>
                    </div>
                </th>
            </tr>
        </thead>
        <tfoot>
            <tr>
                <th>
                    <div class="datagrid-footer-left" style="display: none;">
                        <div class="grid-controls">
                            <span>
                                <span class="grid-start"></span>-<span class="grid-end"></span> of <span class="grid-count"></span>
                            </span>
                            <div class="select grid-pagesize" data-resize="auto">
                                <button type="button" data-toggle="dropdown" class="btn dropdown-toggle">
                                    <span class="dropdown-label"></span>
                                    <span class="caret"></span>
                                </button>
                                <ul class="dropdown-menu">
                                    <li data-value="10"><a href="#">10</a></li>
                                    <li data-value="30" data-selected="true"><a href="#">30</a></li>
                                    <li data-value="50"><a href="#">50</a></li>
                                    <li data-value="100"><a href="#">100</a></li>
                                    <li data-value="500"><a href="#">500</a></li>
                                </ul>
                            </div>
                            <span>items per page</span>
                        </div>
                    </div>
                    <div class="datagrid-footer-right" style="display: none;">
                        <div class="grid-pager">
                            <button type="button" class="btn grid-prevpage"><i class="icon-chevron-left"></i></button>
                            <span>Page</span>
                            <div class="input-append dropdown combobox">
                                <input class="span1" type="text">
                                <button type="button" class="btn" data-toggle="dropdown"><i class="caret"></i>&nbsp;</button>
                                <ul class="dropdown-menu"></ul>
                            </div>
                            <span>of <span class="grid-pages"></span></span>
                            <button type="button" class="btn grid-nextpage"><i class="icon-chevron-right"></i></button>
                        </div>
                    </div>
                </th>
            </tr>
        </tfoot>
    </table>
</div>
<script>
    var payload = <%= Payload %>;
    var dataSource = new StaticDataSource({
        columns: [
            {
                property: "PageName",
                label: "Page name",
                sortable: true
            },
            {
                property: "PageType",
                label: "Page type",
                sortable: true
            },
            {
                property: "Updated",
                label: "Last updated",
                sortable: true
            },
            {
                property: "WriterName",
                label: "Last updated by",
                sortable: true
            },
            {
                property: "CreatorName",
                label: "Created by",
                sortable: true
            },
            {
                property: "Published",
                label: "Published",
                sortable: true
            }
        ],
        formatter: function (items) {
            $.each(items, function (index, item) {                
                item.PageName = "<a href='javascript:UmbClientMgr.mainWindow().openContent(\"" + item.PageId + "\")'>" + item.PageName + "</a>";
            });
        },
        data: payload
    });    
    
    $("#DashboardDatagrid").datagrid({
        dataSource: dataSource,
        dataOptions: {
            pageIndex: 0, 
            pageSize: 30,
            sortProperty: "Updated",
            sortDirection : "desc",
            filter: true
        }
    }).on("loaded", function(e) {
        $(e.target).find("tr:odd").addClass("odd");
    });
</script>
