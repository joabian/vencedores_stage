﻿<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ajustes.aspx.vb" Inherits="reportes_ajustes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.22.custom.min.js" type="text/javascript" ></script>
    <script src="../Scripts/jquery-ui-1.7.1.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/calendar/datepicker.js" type="text/javascript"></script>
    <script src="../Scripts/calendar/datepicker-es.js" type="text/javascript"></script>
    <link href="../Styles/calendar/datepicker.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/redmond/jquery-ui-1.8.22.custom.css" type="text/css" rel="stylesheet" />    

    <script type="text/javascript">


        $(function () {
            var dates = $("#<%=from_date.ClientID%>, #<%=to_date.ClientID%>").datepicker({
                numberOfMonths: 1,
                changeYear: true,
                yearRange: '2010:2200',
                dateFormat: 'mm/dd/yy',
                //  showWeek: true,
                firstDay: 1//,
                //onSelect: function (selectedDate) {
                //var option = this.id == "#<%=from_date.ClientID%>" ? "minDate" : "maxDate",
                //instance = $(this).data("datepicker"),
                //date = $.datepicker.parseDate(
                //instance.settings.dateFormat ||
                //$.datepicker._defaults.dateFormat,
                //selectedDate, instance.settings);
                //dates.not(this).datepicker("option", option, date);
                //}
            });
        });

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <fieldset><legend>Filtros:</legend>
        Desde:
        <asp:TextBox ID="from_date" runat="server"></asp:TextBox>&nbsp&nbsp
        Hasta:
        <asp:TextBox ID="to_date" runat="server"></asp:TextBox><br /><br />
        <asp:Button ID="btn_get_report" runat="server" Text="Generar Reporte" />&nbsp&nbsp&nbsp&nbsp
        <asp:Button ID="btn_export" runat="server" Text="Exportar a Excel" Enabled="false" />
        
    </fieldset>
 
    <asp:GridView ID="gv_results" runat="server">

    </asp:GridView>
    <asp:Label ID="lbl_error" runat="server" Text="" ForeColor="Red"></asp:Label>

    <asp:HiddenField ID="hf_qry" runat="server" />

</asp:Content>

