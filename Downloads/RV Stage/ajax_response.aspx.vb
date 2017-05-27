﻿Imports System.Data

Partial Class ajax_response
    Inherits System.Web.UI.Page
    Public query As String
    Public queryLog As String
    Public Dataconnect As New DataConn_login
    Public ds As DataSet
    Public sendemail As New email_mng
    Dim _OPTION As String
    Dim pedido As String
    Dim itemsAray As String
    Dim location As String
    Dim location_id As String
    Dim pedidoCompleto As String
    Dim qty As String
    Dim item As String
    Dim idItem As String
    Dim cliente As String
    Dim vendedor As String
    Dim urgent As String
    Dim id_item_pedido As String
    Dim newPrice As String
    Dim paqueteria As String
    Dim cajas As String
    Dim codigo_tras As String
    Dim rack As String
    Dim start_date As String
    Dim id_ajuste As String
    Dim comments As String
    Dim transfer As String
    Dim selectedPrice As String
    Dim increaseType As String
    Dim qty_value As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        _REQUEST()

        Dim myOption As String
        myOption = Request.QueryString("option")

        If myOption = "getInventoryNew" Then getInventoryNew()
        If myOption = "uploadItemstoOrder" Then uploadItemstoOrder()
        If _OPTION = "salvarItemsdePedido" Then salvarItemsdePedido()
        If _OPTION = "verificarPedido" Then verificarPedido()
        If _OPTION = "ingresarItemAPedido" Then ingresarItemAPedido()
        If _OPTION = "loadTableItems" Then loadTableItems()
        If _OPTION = "removeItemFromPedido" Then removeItemFromPedido()
        If _OPTION = "SaveOrderInfo" Then SaveOrderInfo()
        If _OPTION = "getLocations" Then getLocations()
        If _OPTION = "getVendedores" Then getVendedores()
        If _OPTION = "loadOrderInfo" Then loadOrderInfo()
        If _OPTION = "editPrice" Then editPrice()
        If _OPTION = "loadTablePedidos" Then loadTablePedidos()
        If _OPTION = "loadTableItemsdePedidosparaSurtir" Then loadTableItemsdePedidosparaSurtir()
        If _OPTION = "loadTableAjustesPendientes" Then loadTableAjustesPendientes()
        If _OPTION = "loadTableClientes" Then loadTableClientes()
        If _OPTION = "resetSession" Then resetSession()
        If _OPTION = "TransferenciaRapida" Then TransferenciaRapida()
        If _OPTION = "getItemInDefaltLocation" Then getItemInDefaltLocation()
        If _OPTION = "loadTableResurtido" Then loadTableResurtido()
        If _OPTION = "getRacks" Then getRacks()
        If _OPTION = "aprobarAjuste" Then aprobarAjuste()
        If _OPTION = "rechazarAjuste" Then rechazarAjuste()
        If _OPTION = "loadTableProductPrices" Then loadTableProductPrices()
        If _OPTION = "salvarNuevoPrecio" Then salvarNuevoPrecio()
        If _OPTION = "salvarPreciosMasivo" Then salvarPreciosMasivo()

    End Sub

    Sub _REQUEST()
        _OPTION = Request("option")

        pedido = Request("pedido")
        itemsAray = Request("itemsArray")
        location = Request("location")
        pedidoCompleto = Request("pedidoCompleto")
        qty = Request("qty")
        item = Request("item")
        idItem = Request("idItem")
        cliente = Request("cliente")
        vendedor = Request("vendedor")
        urgent = Request("urgent")
        id_item_pedido = Request("id_item_pedido")
        newPrice = Request("newPrice")
        paqueteria = Request("paqueteria")
        cajas = Request("cajas")
        codigo_tras = Request("codigo_tras")
        rack = Request("rack")
        location_id = Request("location_id")
        start_date = Request("start_date")
        id_ajuste = Request("id_ajuste")
        comments = Request("comments")
        transfer = Request("transfer")
        selectedPrice = Request("selectedPrice")
        qty_value = Request("qty_value")
        increaseType = Request("increaseType")

    End Sub

    Public Sub salvarPreciosMasivo()
        If selectedPrice = "1" Then
            Dim prices() As String = {"PRECIO_JUAREZ", "PRECIO_DURANGO", "PRECIO_LEON", "PRECIO_TORREON", "PRECIO_MAYOREO_JUAREZ", "PRECIO_2_JUAREZ", "PRECIO_3_JUAREZ", "PRECIO_INSTALADO_JUAREZ", "PRECIO_DLLS_JUAREZ", "precio_rito", "precio_rito2"}
            For Each value As String In prices
                salvarPrecioInd(value)
            Next

            Dim logevent As String = "Incremento de precios masivo (todos los precios) de tipo: " + increaseType + " por la cantidad de: " + qty_value.ToString()
            queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + Membership.GetUser().UserName.ToString() + "', '" + logevent.ToString() + "', getDate())"
            Dataconnect.runquery(queryLog)

        Else
            salvarPrecioInd(selectedPrice)

            Dim logevent As String = "Incremento de precios masivo (" + selectedPrice + ") de tipo: " + increaseType + " por la cantidad de: " + qty_value.ToString()
            queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + Membership.GetUser().UserName.ToString() + "', '" + logevent.ToString() + "', getDate())"
            Dataconnect.runquery(queryLog)
        End If

    End Sub

    Public Sub salvarPrecioInd(ByVal precioSeleccionado As String)
        If increaseType = "porcentual" Then
            query = "update products set " + precioSeleccionado + " = " + precioSeleccionado + " + ( (" + precioSeleccionado + " * " + qty_value.ToString() + ") /100 ) where category = 1 and " + precioSeleccionado + " is not null and " + precioSeleccionado + " > 0"
        ElseIf increaseType = "precio" Then
            query = "update products set " + precioSeleccionado + " = (" + precioSeleccionado + " + " + qty_value + ") where category = 1 and " + precioSeleccionado + " is not null"
        End If
        Dataconnect.runquery(query)
    End Sub

    Public Sub salvarNuevoPrecio()
        query = "update products set " + selectedPrice + " = '" + newPrice.ToString() + "' where code = '" + item.ToString() + "'"
        Dataconnect.runquery(query)

        Dim logevent As String = "Incremento de precio " + selectedPrice + " al codigo: " + item + " nuevo precio: " + newPrice.ToString()
        queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + Membership.GetUser().UserName.ToString() + "', '" + logevent.ToString() + "', getDate())"
        Dataconnect.runquery(queryLog)

    End Sub

    Public Sub loadTableProductPrices()
        Dim htmtable As String = ""
        Dim i, j, m As Integer

        query = "select "
        'query += " top 10"
        query += " products.code as [Codigo]"
        'query += ", products.alias"
        'query += ", categories.name "
        query += ", products.description as [Descripcion]"
        query += ", products.precio_juarez as [Juarez]"
        query += ", products.precio_2_juarez as [Juarez 2]"
        query += ", products.precio_3_juarez as [Juarez 3]"
        query += ", products.precio_mayoreo_juarez as [Juarez Mayoreo]"
        query += ", products.precio_instalado_juarez as [Juarez Instalado]"
        query += ", products.precio_dlls_juarez as [Dolares]"
        query += ", products.precio_durango as [Durango]"
        query += ", products.precio_leon as [Leon]"
        query += ", products.precio_torreon as [Torreon]"
        query += ", products.precio_rito as [Rito]"
        query += ", products.precio_rito2 as [Rito 2]"
        query += " from products"
        query += " inner join categories on products.category = categories.id"
        query += " where products.category = 1"
        query += " order by products.code"
        ds = Dataconnect.GetAll(query)

        If ds.Tables(0).Rows.Count > 0 Then
            'do something

            htmtable += "<table id='tabla_productos' class='tableItems' border='1'><thead>"
            htmtable += "<tr><th>Editar</th>"
            For m = 0 To ds.Tables(0).Columns.Count - 1
                htmtable += "<th>" + ds.Tables(0).Columns(m).ColumnName + "</th>"
            Next

            htmtable += "</tr></thead><tbody>"
            For i = 0 To ds.Tables(0).Rows.Count - 1
                Dim code As String = ds.Tables(0).Rows(i)("Codigo").ToString()
                htmtable += "<tr>"
                htmtable += "<td><center><img src='../images/icons/edit.png' style='cursor:pointer; width:15px;' onclick=""modificarPrecio('" + code.ToString() + "');"" /></center></td>"
                For j = 0 To ds.Tables(0).Columns.Count - 1
                    htmtable += "<td><center>" + ds.Tables(0).Rows(i)(j).ToString() + "</center></td>"
                Next

                htmtable += "</tr>"
            Next

            htmtable += "</tbody></table>"
        Else

        End If

        Response.Write(htmtable)

    End Sub

    Public Sub aprobarAjuste()
        Dim msg As String = ""
        query = "select * from ajustes where id = " + id_ajuste.ToString()
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim qty As String = ds.Tables(0).Rows(0)("qty").ToString()
            Dim tipo As String = ds.Tables(0).Rows(0)("tipo").ToString()
            Dim sucursal As String = ds.Tables(0).Rows(0)("location").ToString()
            Dim item As String = Replace(ds.Tables(0).Rows(0)("item"), " ", "").ToString()
            Dim rack As String = ds.Tables(0).Rows(0)("rack").ToString()

            If tipo = "SALIDA" Then

                query = "select * from stock where product_code = '" + item.ToString() + "' and location = '" + sucursal.ToString() + "' and rack = '" + rack.ToString() + "'"
                query += " and qty >= " + qty.ToString()
                ds = Dataconnect.GetAll(query)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim id_stock As String = ds.Tables(0).Rows(0)("id").ToString()
                    query = "update stock set qty = (qty - " + qty.ToString() + ") where id = " + id_stock.ToString()
                    query += " delete from stock where qty <= 0"

                    Dataconnect.runquery(query)

                    query = "insert into moves (product_id,product_code,reason,type,comments,location,rack,[user],row_date,qty) values (0, '" + item.ToString().ToUpper() + "', 'AJUSTE INVENTARIO', 'SALIDA', '"
                    query += comments.ToString() + "', '" + sucursal.ToString().ToUpper() + "', '"
                    query += rack.ToString().ToUpper() + "', '" + Membership.GetUser().UserName.ToString() + "', getDate(), " + qty.ToString() + ")"
                    Dataconnect.runquery(query)

                    Dim logevent As String = "Salida de producto: " + item.ToString() + " de la sucursal: " + sucursal.ToString() + " del rack: " + rack.ToString() + ", por la cantidad de: " + qty.ToString()
                    queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + Membership.GetUser().UserName.ToString() + "', '" + logevent.ToString() + "', getDate())"
                    Dataconnect.runquery(queryLog)

                    msg = "Ajuste de salida aprobado"

                    query = "update ajustes set approved = 1, approved_user = '" + Membership.GetUser().UserName.ToString() + "', approved_date = getdate()"
                    query += ",resolved_comments = '" + comments.ToString() + "' where id = " + id_ajuste.ToString()
                    Dataconnect.runquery(query)

                Else
                    msg = "No existe suficiente cantidad en el rack y locación especificadas para hacer este ajuste"
                End If

            Else
                query = "select * from stock where product_code = '" + item.ToString() + "' and rack = '" + rack.ToString() + "' and location = '" + sucursal.ToString() + "'"
                ds = Dataconnect.GetAll(query)
                If ds.Tables(0).Rows.Count > 0 Then
                    'sumar a la cantidad actual
                    Dim id_record As String = ds.Tables(0).Rows(0)("id").ToString()
                    
                    query = "update stock set qty = (qty + " + qty.ToString() + ") where id = " + id_record.ToString()
                    Dataconnect.runquery(query)

                Else
                    'ingresar nueva linea
                    query = "insert into stock (product_id,product_code,product_description,product_model,product_low_inventory,product_category,qty,location,last_update,rack,"
                    query += "from_location) select products.id, products.code, products.description, products.model,"
                    query += " products.low_inventory, categories.name, " + qty.ToString() + ", '" + sucursal.ToString().ToUpper()
                    query += "', getDate(), '" + rack.ToString().ToUpper() + "', 'Entrada' from products inner join categories on products.category"
                    query += " = categories.id where products.code = '" + item.ToString() + "'"
                    Dataconnect.runquery(query)

                End If

                query = "insert into moves (product_id,product_code,reason,type,comments,location,rack,[user],row_date,qty) values (0, '" + item.ToString().ToUpper() + "', 'AJUSTE INVENTARIO', 'ENTRADA', '"
                query += comments.ToString() + "', '" + sucursal.ToString().ToUpper() + "', '" + rack.ToString().ToUpper() + "', '"
                query += Membership.GetUser().UserName.ToString() + "', getDate(), " + qty.ToString() + ")"
                Dataconnect.runquery(query)

                Dim logevent As String = "Entrada de producto: " + item.ToString() + " en la sucursal: " + sucursal.ToString().ToUpper() + " al rack: " + rack.ToString().ToUpper() + ", por la cantidad de: " + qty.ToString()
                queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + Membership.GetUser().UserName.ToString() + "', '" + logevent.ToString() + "', getDate())"
                Dataconnect.runquery(queryLog)

                msg = "Ajuste de entrada aprobado"

                query = "update ajustes set approved = 1, approved_user = '" + Membership.GetUser().UserName.ToString() + "', approved_date = getdate()"
                query += ",resolved_comments = '" + comments.ToString() + "' where id = " + id_ajuste.ToString()
                Dataconnect.runquery(query)

            End If

        Else
            msg = "No se encuentra el ajuste, contacte al administrador del sistema"

        End If

        Response.Write(msg)

    End Sub

    Public Sub rechazarAjuste()
        query = "update ajustes set rejected = 1, approved_user = '" + Membership.GetUser().UserName.ToString() + "', rejected_date = getdate()"
        query += ",resolved_comments = '" + comments.ToString() + "' where id = " + id_ajuste.ToString()
        Dataconnect.runquery(query)

        Response.Write("Ajuste Rechazado con exito")

    End Sub

    Public Sub loadTableAjustesPendientes()
        Dim html_table As String = ""
        Dim user As String = Membership.GetUser().UserName

        query = "select id, Item, location as [Sucursal], Rack, Qty, Tipo, notes as [Comentarios], create_date as [Fecha], username as [Usuario]"
        query += " from ajustes"
        query += " where approved is null"
        query += " and rejected is null"

        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            html_table = "<table id='htmlTable' style='width:100%;border-collapse:collapse' border=1><thead><tr>"
            For j = 1 To ds.Tables(0).Columns.Count - 1
                html_table += "<th>" + ds.Tables(0).Columns(j).ColumnName.ToString() + "</th>"
            Next
            html_table += "<th>Resolver</th></tr></thead><tbody>"

            For i = 0 To ds.Tables(0).Rows.Count - 1
                html_table += "<tr>"
                For m = 1 To ds.Tables(0).Columns.Count - 1
                    html_table += "<td>" + ds.Tables(0).Rows(i)(m).ToString() + "</td>"
                Next
                html_table += "<td>"
                If user = "sgonzalez" Or user = "cesar" Or user = "admin" Then
                    html_table += "<img src='../images/icons/edit.png' style='cursor:pointer; width:15px;' onClick='showAjuste(""" + ds.Tables(0).Rows(i)("id").ToString() + """);' />"
                End If

                html_table += "</td></tr>"
            Next
            html_table += "</tbody></table>"
        Else
            html_table = "<h2 style='color:red'>No existen ajustes pendientes</h2>"
        End If

        Response.Write(html_table)


    End Sub

    Public Sub getRacks()
        Dim res As String = "n/a"
        query = "select rack from stock where product_code = '" + codigo_tras.ToString() + "' and qty > 0 and location = '" + location.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            res = "Rack: <select id='ddl_rack_for_trans'>"
            For i = 0 To ds.Tables(0).Rows.Count - 1
                res += "<option value='" + ds.Tables(0).Rows(i)("rack").ToString() + "'>" + ds.Tables(0).Rows(i)("rack").ToString() + "</option>"
            Next
            res += "</select>"
        End If
        Response.Write(res)
    End Sub

    Public Sub loadTableResurtido()
        Dim html_table As String = ""

        query = "select stk.product_code as Código,stk.total_inv as Qty,"
        query += " max_min.min_qty as Mínimo, max_min.max_qty as Máximo, max_min.volumen as Volumen"
        query += " ,enPedidos.total as [Req en Pedidos]"
        query += " ,(max_min.max_qty + isnull(enPedidos.total,0)) - stk.total_inv as 'Piezas faltantes'"

        query += " ,CEILING(((max_min.max_qty + isnull(enPedidos.total,0)) - stk.total_inv)/cast(products.tam_caja as float)) as 'Cajas a Resurtir'"
        query += " from "
        query += " (select product_code, sum(qty) as total_inv, location_id, location from stock where rack = 'ALMACEN' group by product_code, location, location_id) as stk "
        query += " inner join max_min on stk.location_id = max_min.location_id and stk.product_code = max_min.product_code"
        query += " inner join products on stk.product_code = products.code"
        query += " left join (select product_code, sum(qty) as total from sale_order_items inner join sale_order on sale_order.id = sale_order_items.order_id where status in (2,3) group by product_code)"
        query += " enPedidos on stk.product_code = enPedidos.product_code"
        query += " where stk.location_id = " + location_id.ToString() + " and products.tam_caja is not null"
        query += " and max_min.min_qty > stk.total_inv"
        query += " order by max_min.max_qty - stk.total_inv desc"

        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            'GridView1.DataSource = ds.Tables(0)
            'GridView1.DataBind()
            html_table = createTable(ds)
        Else
            'GridView1.DataSource = Nothing
            'GridView1.DataBind()
            html_table = "No data"
        End If

        Response.Write(html_table)

    End Sub

    Public Function createTable(ByVal ds As DataSet) As String
        Dim htmlTable As String = "<table border='1' style='border-collapse:collapse'><tr>"
        For j = 0 To ds.Tables(0).Columns.Count - 1
            htmlTable += "<th>" + ds.Tables(0).Columns(j).ColumnName.ToString() + "</th>"
        Next
        htmlTable += "<th>Locaciones adicionales " + location.ToString() + "</th><th>Otras Sucursales</th><th>Tranferir</th></tr>"
        For i = 0 To ds.Tables(0).Rows.Count - 1
            htmlTable += "<tr>"
            For m = 0 To ds.Tables(0).Columns.Count - 1
                htmlTable += "<td>" + ds.Tables(0).Rows(i)(m).ToString() + "</td>"
            Next
            htmlTable += "<td>"

            Dim prod As String = ds.Tables(0).Rows(i)("Código").ToString()
            query = "select rack, qty from stock where rack <> 'ALMACEN' and product_code = '" + prod.ToString() + "' and location = '" + location.ToString() + "'"
            Dim dsTot As DataSet = Dataconnect.GetAll(query)
            If dsTot.Tables(0).Rows.Count > 0 Then
                For y = 0 To dsTot.Tables(0).Rows.Count - 1
                    htmlTable += dsTot.Tables(0).Rows(y)("rack").ToString.ToUpper + " = " + dsTot.Tables(0).Rows(y)("qty").ToString() + ", "
                Next
            End If
            htmlTable += "</td><td>"

            query = "select location, sum(qty) as tot from stock where product_code = '" + prod.ToString() + "' and location <> '" + location.ToString() + "'"
            query += " group by location"
            dsTot = Dataconnect.GetAll(query)
            If dsTot.Tables(0).Rows.Count > 0 Then
                For y = 0 To dsTot.Tables(0).Rows.Count - 1
                    htmlTable += dsTot.Tables(0).Rows(y)("location").ToString.ToUpper + " = " + dsTot.Tables(0).Rows(y)("tot").ToString() + ", "
                Next
            End If
            htmlTable += "</td>"

            htmlTable += "<td>"
            htmlTable += "<img src='../images/icons/arrowLeft.png' style='cursor:pointer; width:15px;' onClick='showTransferDiv(""" + ds.Tables(0).Rows(i)("Código").ToString() + """);' />"
            htmlTable += "</td>"
            htmlTable += "</tr>"


        Next
        htmlTable += "</table>"

        Return htmlTable

    End Function


    Public Sub getItemInDefaltLocation()
        Dim to_rack As String = "ALMACEN"
        Dim msg As String = ""
        query = "select rack from default_locators where location = '" + location.ToString() + "' and code = '" + codigo_tras.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            to_rack = ds.Tables(0).Rows(0)("rack").ToString()
        Else
            query = "select count(*) as tot, rack from default_locators where location = '" + location.ToString() + "' group by rack order by tot desc"
            ds = Dataconnect.GetAll(query)
            If ds.Tables(0).Rows.Count > 0 Then
                to_rack = ds.Tables(0).Rows(0)("rack").ToString()
            End If
        End If
        query = "select qty from stock where product_code = '" + codigo_tras.ToString()
        query += "' and location = '" + location.ToString() + "' and rack = '" + to_rack.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            msg = ds.Tables(0).Rows(0)("qty").ToString()
        Else
            msg = "0"
        End If

        Response.Write(msg)

    End Sub

    Public Sub TransferenciaRapida()
        Dim to_rack As String = "ALMACEN"
        Dim msg As String = ""
        query = "select rack from default_locators where location = '" + location.ToString() + "' and code = '" + codigo_tras.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            to_rack = ds.Tables(0).Rows(0)("rack").ToString()
        Else
            query = "select count(*) as tot, rack from default_locators where location = '" + location.ToString() + "' group by rack order by tot desc"
            ds = Dataconnect.GetAll(query)
            If ds.Tables(0).Rows.Count > 0 Then
                to_rack = ds.Tables(0).Rows(0)("rack").ToString()
                query = "insert into default_locators (code, location, rack) values ('" + codigo_tras.ToString() + "','" + location.ToString() + "','" + to_rack.ToString() + "')"
                Dataconnect.runquery(query)
            End If
        End If

        query = "select id, qty from stock where product_code = '" + codigo_tras.ToString()
        query += "' and location = '" + location.ToString() + "' and rack = '" + rack.ToString() + "' and qty >= '" + qty.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim id_from As String = ds.Tables(0).Rows(0)("id").ToString()

            query = "select id from stock where product_code = '" + codigo_tras.ToString()
            query += "' and location = '" + location.ToString() + "' and rack = '" + to_rack.ToString() + "'"
            ds = Dataconnect.GetAll(query)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim id As String = ds.Tables(0).Rows(0)("id").ToString()
                query = "update stock set qty = (qty + " + qty.ToString() + ") where id = " + id.ToString()
                Dataconnect.runquery(query)

            Else
                query = "insert into stock (product_id,product_code,product_description,product_model,product_low_inventory,product_category"
                query += ",qty,location,last_update,rack,from_location,location_id) select products.id, products.code, products.description,"
                query += " products.model,products.low_inventory, categories.name, " + qty.ToString() + ", '" + location.ToString()
                query += "', getDate(), '" + to_rack.ToString().ToUpper() + "', 'PEDIDO', locations.id"
                query += " from products inner join categories on products.category = categories.id"
                query += " inner join locations ON locations.alias = '" + location.ToString() + "'"
                query += " where products.code = '" + codigo_tras.ToString() + "'"
                Dataconnect.runquery(query)

            End If

            query = "update stock set qty = (qty - " + qty.ToString() + ") where id = " + id_from.ToString()
            query += " delete from stock where qty <= 0"
            Dataconnect.runquery(query)
            Dim logevent As String = "Transfirio desde un pedido un producto de la sucursal: " + location.ToString() + " en el rack: " + to_rack.ToString() + ", producto: " + codigo_tras.ToString() + ", cantidad: " + qty.ToString()

            queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + Membership.GetUser().UserName.ToString() + "', '" + logevent.ToString() + "', getDate())"
            Dataconnect.runquery(queryLog)

            msg = "Transferencia realizada con exito!"
        Else
            msg = "No hay suficiente cantidad en el rack especificado"
        End If

        Response.Write(msg)
    End Sub

    Public Sub resetSession()
        Dim user, htmtable As String
        htmtable = ""
        user = Membership.GetUser().UserName

        query = "select location from users where user_name = '" + user + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables.Count > 0 Then
            If ds.Tables(0).Rows.Count > 0 Then
                htmtable = "Data"
            Else
                htmtable = "No data"
            End If
        End If
        Response.Write(htmtable)
    End Sub

    Public Sub loadTableClientes()

        Dim user, htmtable As String
        htmtable = ""
        user = Membership.GetUser().UserName
        query = "select location from users where user_name = '" + user + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim location As String = ds.Tables(0).Rows(0)("location").ToString()
            Dim location_st As String = ""
            If location <> "0" Then
                location_st = " and location = " + location.ToString()
            End If

            query = "select id, name, bill_address, telephone, paqueteria, serv_paq, flete from clients where active = 1"
            query += location_st.ToString()
            query += " order by name"
            ds = Dataconnect.GetAll(query)

            If ds.Tables(0).Rows.Count > 0 Then
                'do something

                htmtable += "<table id='tabla_clientes' class='tableItems' border='1'><thead>"
                htmtable += "<tr><th>Editar</th><th>Nombre</th><th>Dirección</th><th>Teléfono</th><th>Paquetería</th><th>Servicio</th><th>Flete</th></tr></thead><tbody>"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    Dim id_assignment As String = ds.Tables(0).Rows(i)("id").ToString()
                    htmtable += "<tr>"
                    htmtable += "<td><center><a href='nuevo_cliente.aspx?cliente=" + id_assignment.ToString() + "'>Editar</a></center></td>"
                    For j = 1 To ds.Tables(0).Columns.Count - 1
                        htmtable += "<td><center>" + ds.Tables(0).Rows(i)(j).ToString() + "</center></td>"
                    Next

                    htmtable += "</tr>"
                Next

                htmtable += "</tbody></table>"
            Else

            End If
        Else
            'No data

        End If



        Response.Write(htmtable)

    End Sub

    Public Sub editPrice()
        query = "update sale_order_items set sold_price = '" + newPrice.ToString() + "'"
        query += " where id = '" + id_item_pedido.ToString() + "'"
        Dataconnect.runquery(query)
        Response.Write("Precio actualizado!")
    End Sub

    Public Sub loadOrderInfo()

        Dim locs As String = "no data"
        query = "select convert(varchar, clients.id, 1) + ' - ' + clients.name as customer, sale_order.location, vendor, urgent, transfer "
        query += " from sale_order inner join clients on clients.id = sale_order.customer"
        query += " where sale_order.id = '" + pedido.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            locs = ds.Tables(0).Rows(0)("customer").ToString() + "}" + ds.Tables(0).Rows(0)("location").ToString() + "}" + ds.Tables(0).Rows(0)("vendor").ToString() + "}" + ds.Tables(0).Rows(0)("urgent").ToString() + "}" + ds.Tables(0).Rows(0)("transfer").ToString()
        End If

        Response.Write(locs)

    End Sub

    Public Sub getVendedores()
        Dim locs As String = ""
        Dim user As String
        user = Membership.GetUser().UserName
        query = "select location from users where user_name = '" + user + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim location As String = ds.Tables(0).Rows(0)("location").ToString()
            If location = "0" Then
                location = ""
            Else
                location = " and location = " + location.ToString()
            End If

            query = "select id, name + ' ' + last_name as emp from employees where position = 'vendedor' and active = 1"
            query += location

            ds = Dataconnect.GetAll(query)
            If ds.Tables(0).Rows.Count > 0 Then
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    locs += ds.Tables(0).Rows(i)("id").ToString() + "}" + ds.Tables(0).Rows(i)("emp").ToString() + "]"
                Next
            End If

        Else
            'No data

        End If


        Response.Write(locs)

    End Sub


    Public Sub getLocations()
        Dim locs As String = ""
        query = "Select id, alias from locations where alias <> 'Pedido'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            For i = 0 To ds.Tables(0).Rows.Count - 1
                locs += ds.Tables(0).Rows(i)("id").ToString() + "}" + ds.Tables(0).Rows(i)("alias").ToString() + "]"
            Next
        End If

        Response.Write(locs)

    End Sub


    Public Sub SaveOrderInfo()
        Dim clienteArr As String()
        clienteArr = cliente.Split("-")

        query = "update sale_order set vendor = '" + vendedor.ToString() + "'"
        query += ", location = '" + location.ToString() + "'"
        query += ", customer = '" + Trim(clienteArr(0)).ToString() + "'"
        query += ", urgent = '" + urgent.ToString() + "'"
        query += ", paqueteria = '" + paqueteria.ToString() + "'"
        query += ", transfer = '" + transfer.ToString() + "'"
        query += " where id = '" + pedido.ToString() + "'"
        Dataconnect.runquery(query)

    End Sub

    Public Sub verificarPedido()
        Dim order As String = pedido
        Dim username As String
        Dim logevent As String

        username = Membership.GetUser().UserName
        query = "update sale_order set status = 8 where id = " + order.ToString()
        Dataconnect.runquery(query)

        logevent = "Actualizacion de pedido: " + order.ToString() + " nuevo status: Verificado"
        queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + username.ToString() + "', '" + logevent.ToString() + "', getDate())"
        Dataconnect.runquery(queryLog)
        Response.Write("Pedido verificado con exito!")
    End Sub

    Public Sub loadTableItems()
        Dim table As String = ""
        query = "select sale_order_items.id, product_code as [Codigo], description as [Descripcion], qty as [Cantidad],"
        query += " sold_price as [Precio Unitario], (qty * sold_price) as [Total]"
        query += " from sale_order_items inner join products on products.code = sale_order_items.product_code"
        query += " where order_id = '" + pedido.ToString() + "' and active = 1 order by update_date desc"
        ds = Dataconnect.GetAll(query)

        If ds.Tables(0).Rows.Count > 0 Then
            table += "<table class='tableItems' border='1'>"
            table += "<tr><th>Borrar</th><th>Codigo</th><th>Descripcion</th><th>Cantidad</th><th>P.U.</th><th>P.Total</th></tr>"
            For i = 0 To ds.Tables(0).Rows.Count - 1
                Dim id_assignment As String = ds.Tables(0).Rows(i)("id").ToString()
                table += "<tr>"
                table += "<td><a href='#' onclick='DeleteItem(" + ds.Tables(0).Rows(i)("id").ToString() + ");'>"
                table += "<img alt='borrar' src='../images/icons/gnome_edit_delete.png' style='width:30px' /></a></td>"
                For j = 1 To ds.Tables(0).Columns.Count - 1
                    If j = 4 Then
                        table += "<td><span id='label_" + id_assignment + "'>" + ds.Tables(0).Rows(i)(j).ToString() + "</span>&nbsp;&nbsp;&nbsp;"
                        table += "<img id='img_" + id_assignment + "' src='../images/icons/edit.png' style='cursor:pointer;width:25px' onClick='editDate(" + id_assignment + ")' />"
                        table += "<input type='textbox' style='width:100px' id='new_date_" + id_assignment + "' class='date_hide' />"
                        table += "<img id='img_save_" + id_assignment + "' alt='Save new due date' class='date_hide' src='../images/icons/ok.png' style='cursor:pointer;width:25px' onClick='saveNewDate(" + id_assignment + ");loatTableItems();' />"
                        table += "</td>"
                    Else
                        table += "<td>" + ds.Tables(0).Rows(i)(j).ToString() + "</td>"
                    End If
                    'table += "<td>" + ds.Tables(0).Rows(i)(j).ToString() + "</td>"
                Next
                table += "</tr>"
            Next

            table += "</table>"
        Else

        End If

        Response.Write(table)
    End Sub

    Public Sub loadTablePedidos()
        Dim table As String = ""
        Dim user As String
        user = Membership.GetUser().UserName
        query = "select location from users where user_name = '" + user + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim location As String = ds.Tables(0).Rows(0)("location").ToString()
            Dim location_st As String = ""
            If location <> "0" Then
                location_st = " and sale_order.location = " + location.ToString()
            End If

            query = "select sale_order.id, clients.name as customer,"
            query += " order_status.status, sale_order.ship_date, tots.tot, case when sale_order.urgent = 1 then 'Urgente' else '' end as urgent from sale_order"
            query += " inner join clients on sale_order.customer = clients.id "
            query += " inner join order_status on sale_order.status = order_status.id"
            query += " inner join (select sum(qty) as tot, order_id from sale_order_items where active = 1 group by order_id) tots"
            query += " on sale_order.id = tots.order_id"
            query += " where sale_order.status in (2,3) " + location_st.ToString()
            query += " order by sale_order.urgent desc, sale_order.id"
            ds = Dataconnect.GetAll(query)

            If ds.Tables(0).Rows.Count > 0 Then
                'do something

                table += "<table class='tableItems' border='1'>"
                table += "<tr><th>Orden</th><th>Cliente</th><th>Estatus</th><th>Fecha de Envío</th><th>Total de Piezas</th><th>Urgente</th><th>Surtir</th></tr>"
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    Dim id_assignment As String = ds.Tables(0).Rows(i)("id").ToString()
                    table += "<tr>"
                    For j = 0 To ds.Tables(0).Columns.Count - 1
                        table += "<td>" + ds.Tables(0).Rows(i)(j).ToString() + "</td>"
                    Next
                    table += "<td><a href='surtir.aspx?order=" + id_assignment.ToString() + "'>Surtir</a></td>"
                    table += "</tr>"
                Next

                table += "</table>"
            Else

            End If
        Else
            'No data

        End If



        Response.Write(table)
    End Sub

    Public Sub loadTableItemsdePedidosparaSurtir()
        Dim html_table As String = ""

        query = "select pedido.product_code as [Codigo], pedido.qty as [Cantidad Pedida], pedido.qty_picked as "
        query += " [Cantidad Surtida], (pedido.qty - pedido.qty_picked) as [Cantidad faltante] ,isnull(stock.qty, 0)"
        query += " as [Cantidad en locacion default],isnull(totales_suc.qty,0) as [Total en " + location.ToString() + "],"
        query += " isnull(totales.qty,0) as [Total en Todas las Sucursales], max_min.Volumen"
        query += " ,case when max_min.Volumen = 'ALTO' then '1' when max_min.Volumen = 'REGULAR' then '2' when max_min.Volumen = 'BAJO' then 3 end as ord"
        query += " from sale_order_items as pedido "
        query += " left join default_locators as locators on pedido.product_code = locators.code and locators.location = '" + location.ToString() + "'"
        query += " left join stock on pedido.product_code = stock.product_code and stock.location = '" + location.ToString() + "'"
        query += " and stock.rack = locators.rack "
        query += " left join (select product_code, sum(qty) as qty from stock where location = '" + location.ToString() + "'"
        query += " group by product_code) as totales_suc on pedido.product_code = totales_suc.product_code"
        query += " inner join sale_order on sale_order.id = pedido.order_id"
        query += " left join max_min on pedido.product_code = max_min.product_code"
        query += " and max_min.location_id = sale_order.location"
        query += " left join (select product_code, sum(qty) as qty from stock group by product_code) as totales on pedido.product_code = "
        query += " totales.product_code where pedido.active = 1 and pedido.order_id = " + pedido.ToString() + " order by ord, pedido.product_code"

        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            html_table = "<table id='htmlTable' style='width:100%;border-collapse:collapse' border=1><thead><tr>"
            For j = 0 To ds.Tables(0).Columns.Count - 1
                html_table += "<th>" + ds.Tables(0).Columns(j).ColumnName.ToString() + "</th>"
            Next
            html_table += "<th>trans</th></tr></thead><tbody>"

            For i = 0 To ds.Tables(0).Rows.Count - 1
                html_table += "<tr>"
                For m = 0 To ds.Tables(0).Columns.Count - 1
                    html_table += "<td>" + ds.Tables(0).Rows(i)(m).ToString() + "</td>"
                Next
                html_table += "<td>"
                html_table += "<img src='../images/icons/arrowLeft.png' style='cursor:pointer; width:15px;' onClick='showTransferDiv(""" + ds.Tables(0).Rows(i)("Codigo").ToString() + """);' />"
                html_table += "</td></tr>"
            Next
            html_table += "</tbody></table>"
        End If

        Response.Write(html_table)
    End Sub

    Public Sub removeItemFromPedido()
        query = "delete from sale_order_items where id = " + idItem.ToString()
        Dataconnect.runquery(query)

    End Sub

    Public Sub ingresarItemAPedido()
        Dim resp As String = ""
        Dim price As String
        query = "select default_price, name from sale_order inner join clients on sale_order.customer = clients.id where sale_order.id = " + pedido.ToString()
        ds = Dataconnect.GetAll(query)

        If ds.Tables(0).Rows.Count > 0 Then
            price = ds.Tables(0).Rows(0)("default_price").ToString()
            If price = "" Then
                price = "0"
            End If
        Else
            price = "0"
        End If

        'verificamos que sean cantidades numericas
        Dim id_record As String

        If Not IsNumeric(qty) Or Not IsNumeric(price) Then
            'error_msg += "Campo de Cantidad esta incorrecto para producto: " + item.ToString() + " en linea: " + (i + 1).ToString() + "<br />"
        Else
            query = "select * from products where code = '" + item.ToString() + "'"
            ds = Dataconnect.GetAll(query)
            If ds.Tables(0).Rows.Count > 0 Then
                'verificamos si existe una linea en la orden, para sumarla a la cantidad actual
                query = "select * from sale_order_items where active = 1 and order_id = " + pedido.ToString() + " and product_code = '" + item.ToString() + "'"
                ds = Dataconnect.GetAll(query)
                If ds.Tables(0).Rows.Count > 0 Then
                    'sumar a la cantidad actual
                    id_record = ds.Tables(0).Rows(0)("id").ToString()
                    query = "update sale_order_items set update_date = getdate(), qty = (qty + " + qty.ToString() + ") where id = " + id_record.ToString()
                    Dataconnect.runquery(query)
                Else
                    'ingresar nueva linea
                    query = "insert into sale_order_items (order_id,qty,sold_price,product_code,qty_picked,active,update_date) values ("
                    query += pedido.ToString() + ", " + qty.ToString() + ", " + price.ToString() + ", '" + item.ToString().ToUpper()
                    query += "', 0, 1,getdate())"
                    Dataconnect.runquery(query)
                End If
                resp = "ok"
            Else
                resp = "Item inexistente, no se ingreso al pedido"
            End If
        End If

        Response.Write(resp)

    End Sub

    Public Sub salvarItemsdePedido()
        Dim allitems() As String = itemsAray.Split("]")
        Dim order As String = pedido
        Dim username As String
        Dim logevent As String

        username = Membership.GetUser().UserName
        query = "select status from sale_order where id = " + order.ToString()
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim status As String = ds.Tables(0).Rows(0)("status").ToString()
            If status = "4" Then
                Response.Write("Pedido guardado con exito")
            Else
                query = "update sale_order set start_date = '" + start_date.ToString() + "' where id = " + order.ToString() + " and start_date is null"
                query += " update sale_order set status = 2, cajas = '" + cajas.ToString() + "' where id = " + order.ToString()
                Dataconnect.runquery(query)

                logevent = "Actualizacion de pedido: " + order.ToString() + " nuevo status: Preparando"
                queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + username.ToString() + "', '" + logevent.ToString() + "', getDate())"
                Dataconnect.runquery(queryLog)


                If pedidoCompleto = "1" Then
                    For i = 0 To allitems.Length - 2
                        Dim iteminfo() As String = allitems(i).Split("}")
                        saveItemCompleto(Replace(iteminfo(0), " ", ""), iteminfo(1))
                    Next

                    'pedidos duplicados
                    query = "update sale_order_items set qty_picked = qty where qty_picked = (qty * 2)"
                    Dataconnect.runquery(query)

                    'negadas
                    query = "select id, (qty - qty_picked) as negadas from sale_order_items where order_id = '" + pedido.ToString() + "'"
                    ds = Dataconnect.GetAll(query)
                    If ds.Tables(0).Rows.Count > 0 Then
                        For j = 0 To ds.Tables(0).Rows.Count - 1
                            Dim myid As Integer = ds.Tables(0).Rows(j)("id").ToString()
                            Dim neg As Integer = Convert.ToInt32(ds.Tables(0).Rows(j)("negadas").ToString())
                            If neg > 0 Then
                                query = "insert into negadas (codigo,categoria,descripcion,sucursal,notas,qty_req,qty_suc,qty_tot,existe,usuario,row_date)"
                                query += " select sale_order_items.product_code as codigo, categories.name, products.description, '" + location.ToString().ToUpper() + "' as locacion,"
                                query += " 'Negada automatica del pedido " + order.ToString() + "' as notas,'" + neg.ToString() + "' as qty_req,0 as qty_suc,0 as qty_tot,'Si' as existe,"
                                query += " '" + username.ToString() + "' as usuario,getdate() as row_date from sale_order_items"
                                query += " inner join products on sale_order_items.product_code = products.code"
                                query += " left join categories on products.category = categories.id"
                                query += " where sale_order_items.id = '" + myid.ToString() + "'"

                                Dataconnect.runquery(query)

                            End If

                        Next


                    End If

                    query = "update sale_order set status = 4 where id = " + order.ToString()
                    Dataconnect.runquery(query)

                    logevent = "Actualizacion de pedido: " + order.ToString() + " nuevo status: Surtido Completo"
                    queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + username.ToString() + "', '" + logevent.ToString() + "', getDate())"
                    Dataconnect.runquery(queryLog)

                    Dim body As String = bodyHtml()
                    Dim distro As String = "henequenbodegados@gmail.com,rito.hernandez.vencedores@hotmail.com,vencedores_ventas@hotmail.com"

                    sendemail.sendEmail(distro, "Pedido Surtido", body)

                    Response.Write("Pedido guardado con exito")

                Else

                    For i = 0 To allitems.Length - 2
                        Dim iteminfo() As String = allitems(i).Split("}")
                        saveItem(Replace(iteminfo(0), " ", ""), iteminfo(1))
                    Next

                    query = "update sale_order set start_date = '" + start_date.ToString() + "' where id = " + order.ToString() + " and start_date is null"
                    Dataconnect.runquery(query)

                    query = "select sum(qty_picked) as picked from sale_order_items where active =1 and order_id = " + order.ToString()
                    query += "having sum(qty_picked) = sum(qty)"

                    ds = Dataconnect.GetAll(query)
                    If ds.Tables(0).Rows.Count > 0 Then

                        ' erificar si esto aplica de negadas por ser un pedido parcial 
                        query = "select id, (qty - qty_picked) as negadas from sale_order_items where order_id = '" + pedido.ToString() + "'"
                        ds = Dataconnect.GetAll(query)
                        If ds.Tables(0).Rows.Count > 0 Then
                            For j = 0 To ds.Tables(0).Rows.Count - 1
                                Dim myid As Integer = ds.Tables(0).Rows(j)("id").ToString()
                                Dim neg As Integer = Convert.ToInt32(ds.Tables(0).Rows(j)("negadas").ToString())
                                If neg > 0 Then
                                    query = "insert into negadas (codigo,categoria,descripcion,sucursal,notas,qty_req,qty_suc,qty_tot,existe,usuario,row_date)"
                                    query += " select sale_order_items.product_code as codigo, categories.name, products.description, '" + location.ToString().ToUpper() + "' as locacion,"
                                    query += " 'Negada automatica del pedido " + order.ToString() + "' as notas,'" + neg.ToString() + "' as qty_req,0 as qty_suc,0 as qty_tot,'Si' as existe,"
                                    query += " '" + username.ToString() + "' as usuario,getdate() as row_date from sale_order_items"
                                    query += " inner join products on sale_order_items.product_code = products.code"
                                    query += " left join categories on products.category = categories.id"
                                    query += " where sale_order_items.id = '" + myid.ToString() + "'"

                                    Dataconnect.runquery(query)

                                End If

                            Next


                        End If

                        'do something
                        query = "update sale_order set status = 4 where id = " + order.ToString()
                        Dataconnect.runquery(query)

                        logevent = "Actualizacion de pedido: " + order.ToString() + " nuevo status: Surtido Completo"
                        queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + username.ToString() + "', '" + logevent.ToString() + "', getDate())"
                        Dataconnect.runquery(queryLog)

                        Dim body As String = bodyHtml()
                        Dim distro As String = "henequenbodegados@gmail.com,rito.hernandez.vencedores@hotmail.com"

                        sendemail.sendEmail(distro, "Pedido Surtido", body)

                        Response.Write("Pedido guardado con exito")

                    Else
                        'No data
                    End If


                End If

            End If




        Else
            'No data
        End If





    End Sub

    Public Sub saveItem(ByVal item As String, ByVal qty As String)

        Dim order As String = pedido
        Dim sucursal As String = location

        query = "select * from products left join default_locators loc on products.code = loc.code and loc.location = '" + sucursal.ToString() + "' where products.code = '" + item.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim default_locator As String = ds.Tables(0).Rows(0)("rack").ToString()
            If default_locator = "" Then
                '    lbl_error.Text = "Este producto no tiene una locacion por default, corrija este dato antes de continuar"
            Else
                query = "select * from sale_order_items where order_id = " + order.ToString() + " and product_code = '"
                query += item.ToString() + "' and active = 1"
                ds = Dataconnect.GetAll(query)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim order_item_id As String = ds.Tables(0).Rows(0)("id").ToString()

                    query = "select top 1 * from stock where product_code = '" + item.ToString() + "' and location = '" + sucursal.ToString() + "'"
                    query += " and qty > 0 and rack = '" + default_locator + "'"
                    ds = Dataconnect.GetAll(query)
                    If ds.Tables(0).Rows.Count > 0 Then
                        'do something
                        Dim stock_id As String = ds.Tables(0).Rows(0)("id").ToString()
                        Dim rack As String = default_locator 'ds.Tables(0).Rows(0)("rack").ToString()
                        Dim product_id As String = ds.Tables(0).Rows(0)("product_id").ToString().ToUpper()

                        query = " update sale_order_items set update_date = getdate(), qty_picked = (qty_picked + " + qty.ToString() + ") where id = " + order_item_id.ToString()
                        Dataconnect.runquery(query)

                        Dim username As String
                        Dim logevent As String
                        username = Membership.GetUser().UserName

                        query = "insert into moves (product_id,product_code,reason,type,comments,location,rack,[user],row_date,qty) values (" + product_id.ToString().ToUpper() + ", '" + item.ToString().ToUpper() + "', 'VENTA', 'SALIDA', 'salida por surtido de pedido " + order.ToString() + "', '" + sucursal.ToString() + "', '" + rack.ToString() + "', '" + username.ToString() + "', getDate(), " + qty.ToString() + ")"

                        Dataconnect.runquery(query)

                        logevent = "Surtido de pedido: " + order.ToString() + " producto: " + item.ToString() + " de la sucursal: " + sucursal.ToString() + " del rack: " + rack.ToString()
                        queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + username.ToString() + "', '" + logevent.ToString() + "', getDate())"
                        Dataconnect.runquery(queryLog)

                    Else
                        'Sound.Play()

                    End If
                Else
                    'Sound.Play()

                End If
            End If
            'do something

        Else

        End If

    End Sub


    Public Sub saveItemCompleto(ByVal item As String, ByVal qty As String)

        Dim order As String = pedido
        Dim sucursal As String = location
        Dim default_locator As String = "ALMACEN"

        query = "select rack from default_locators where location = '" + sucursal.ToString() + "' and code = '" + item.ToString() + "'"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            default_locator = ds.Tables(0).Rows(0)("rack").ToString()
        Else
            query = "select count(*) as tot, rack from default_locators where location = '" + sucursal.ToString() + "' group by rack order by tot desc"
            ds = Dataconnect.GetAll(query)
            If ds.Tables(0).Rows.Count > 0 Then
                default_locator = ds.Tables(0).Rows(0)("rack").ToString()
            End If
        End If

        query = "select * from sale_order_items where order_id = " + order.ToString() + " and product_code = '"
        query += item.ToString() + "' and active = 1"
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim order_item_id As String = ds.Tables(0).Rows(0)("id").ToString()

            query = "select top 1 * from stock where product_code = '" + item.ToString() + "' and location = '" + sucursal.ToString() + "'"
            query += " and qty > 0 and rack = '" + default_locator + "'"
            ds = Dataconnect.GetAll(query)
            If ds.Tables(0).Rows.Count > 0 Then
                'do something
                Dim stock_id As String = ds.Tables(0).Rows(0)("id").ToString()
                Dim rack As String = default_locator 'ds.Tables(0).Rows(0)("rack").ToString()
                Dim product_id As String = ds.Tables(0).Rows(0)("product_id").ToString()

                query = "update stock set qty = (qty - " + qty.ToString() + ") where id = " + stock_id.ToString()
                query += " update sale_order_items set update_date = getdate(), qty_picked = (qty_picked + " + qty.ToString() + ") where id = " + order_item_id.ToString()
                query += " delete from stock where qty <= 0"
                Dataconnect.runquery(query)

                Dim username As String
                Dim logevent As String
                username = Membership.GetUser().UserName

                query = "insert into moves (product_id,product_code,reason,type,comments,location,rack,[user],row_date,qty)"
                query += "  values (" + product_id.ToString() + ", '" + item.ToString().ToUpper() + "', 'VENTA', 'SALIDA', "
                query += " 'salida por surtido de pedido " + order.ToString() + "', '" + sucursal.ToString() + "', '"
                query += rack.ToString() + "', '" + username.ToString() + "', getDate(), " + qty.ToString() + ")"
                Dataconnect.runquery(query)

                logevent = "Surtido de pedido: " + order.ToString() + " producto: " + item.ToString() + " de la sucursal: " + sucursal.ToString() + " del rack: " + rack.ToString()
                queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + username.ToString() + "', '" + logevent.ToString() + "', getDate())"
                Dataconnect.runquery(queryLog)

            Else

                'Sound.Play()
            End If
        Else
            'Sound.Play()

        End If

        'do something



    End Sub

    Public Sub uploadItemstoOrder()
        Dim list_items, order_number, item, qty, price, error_msg, id_record As String
        error_msg = ""
        Dim item_details() As String
        order_number = Request.QueryString("order_number")
        list_items = Request.QueryString("items")
        list_items = list_items.Substring(0, list_items.Length - 1)
        Dim items_array() As String = list_items.Split("]")
        For i = 0 To items_array.Length - 1

            item_details = items_array(i).Split("}")
            item = Replace(item_details(0).ToString(), "'", "").ToUpper()
            qty = item_details(1).ToString()
            price = item_details(2).ToString()

            'verificamos que sean cantidades numericas
            If Not IsNumeric(qty) Or Not IsNumeric(price) Then
                error_msg += "Campo de Cantidad o Precio estan incorrectos en linea: " + (i + 1).ToString() + "<br />"
            Else
                query = "select * from products where code = '" + item.ToString() + "'"
                ds = Dataconnect.GetAll(query)
                If ds.Tables(0).Rows.Count > 0 Then
                    'verificamos si existe una linea en la orden, para sumarla a la cantidad actual
                    query = "select * from sale_order_items where active = 1 and order_id = " + order_number.ToString() + " and product_code = '" + item.ToString() + "'"
                    ds = Dataconnect.GetAll(query)
                    If ds.Tables(0).Rows.Count > 0 Then
                        'sumar a la cantidad actual
                        id_record = ds.Tables(0).Rows(0)("id").ToString()
                        query = "update sale_order_items set qty = (qty + " + qty.ToString() + ") where id = " + id_record.ToString()
                        Dataconnect.runquery(query)
                    Else
                        'ingresar nueva linea
                        query = "insert into sale_order_items (order_id,qty,sold_price,product_code,qty_picked,active) values ("
                        query += order_number.ToString() + ", " + qty.ToString() + ", " + price.ToString() + ", '" + item.ToString().ToUpper()
                        query += "', 0, 1)"
                        Dataconnect.runquery(query)
                    End If
                Else
                    error_msg += "El item: " + item.ToString() + " no existe en la base de datos<br />"
                End If
            End If
        Next
        If error_msg <> "" Then
            Response.Write("Los siguiente errores fueron detectados:<br />" + error_msg)
        Else
            Response.Write("El archivo se cargo con exito!")
        End If
    End Sub

    Public Sub getInventory()
        Dim code, html As String
        code = Request.QueryString("code")
        query = "select top 10 product_code, location, sum(qty) as total from stock where qty > 0"
        query += " and product_code like '%" + code.ToString() + "%'"
        query += " group by product_code, location order by total desc"

        ds = Dataconnect.GetAll(query)
        html = "<div style='text-align:center; width:200px; border:1px dotted blue; background-color:#f8e36e; z-index:100; position:absolute;'>"
        If ds.Tables(0).Rows.Count > 0 Then
            html += "<table><tr><th>Codigo</th><th>locacion</th><th>Cantidad</th><tr>"
            For i = 0 To ds.Tables(0).Rows.Count - 1
                html += "<tr style='text-align:center;'><td>" + ds.Tables(0).Rows(i)("product_code").ToString() + "</td>"
                html += "<td>" + ds.Tables(0).Rows(i)("location").ToString() + "</td>"
                html += "<td>" + ds.Tables(0).Rows(i)("total").ToString() + "</td></tr>"
            Next
            html += "</table>"
        Else
            html += "No existen coincidencias"
        End If
        html += "</div>"

        Response.Write(html)
    End Sub

    Public Sub getInventoryNew()
        Dim code, total, location As String
        code = Request.QueryString("code")
        location = Request.QueryString("location")

        query = "select sum(qty) as total from stock "
        query += "inner join locations on stock.location = locations.alias"
        query += " where product_code = '" + code.ToString() + "' and locations.id = '" + location.ToString() + "'"

        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            total = ds.Tables(0).Rows(0)("total").ToString()
            If total = "" Then total = "0"
        Else
            total = "0"
        End If

        Response.Write(total)

    End Sub

    Function bodyHtml() As String
        Dim html As String = ""

        html += "<h1>Notificacion</h1><br />"
        html += "<h2>Cambio de estatus de un pedido</h2><br />"
        html += "<h2>Numero de Pedido: <b>" + pedido.ToString() + "</b><br />"
        html += "Nuevo status: <b>Surtido Completo</b><br /></h2>"

        query = "select sale_order.shipping_address, contact_info, convert(varchar, date, 101) as fecha, convert(varchar, rsd, 101) as rsd,"
        query += " case when ship_date is null then 'N/A' else convert(varchar, ship_date, 101) end as shipped, clients.name as cliente,"
        query += " employees.name + ' ' + employees.last_name as vendedor"
        query += " from sale_order"
        query += " inner join clients on sale_order.customer = clients.id"
        query += " inner join employees on sale_order.vendor = employees.id"
        query += " where sale_order.id = " + pedido.ToString()
        ds = Dataconnect.GetAll(query)
        If ds.Tables(0).Rows.Count > 0 Then
            Dim cliente, ship_address, contact, fecha, rsd, envio_fecha, vendedor As String
            cliente = ds.Tables(0).Rows(0)("cliente").ToString()
            ship_address = ds.Tables(0).Rows(0)("shipping_address").ToString()
            contact = ds.Tables(0).Rows(0)("contact_info").ToString()
            fecha = ds.Tables(0).Rows(0)("fecha").ToString()
            rsd = ds.Tables(0).Rows(0)("rsd").ToString()
            envio_fecha = ds.Tables(0).Rows(0)("shipped").ToString()
            vendedor = ds.Tables(0).Rows(0)("vendedor").ToString()

            html += "<h3><b>Informacion del pedido</b><br />"
            html += "Cliente: <b>" + cliente.ToString() + "</b><br />"
            html += "Contacto: <b>" + contact.ToString() + "</b><br />"
            html += "Vendedor: <b>" + vendedor.ToString() + "</b><br />"
            html += "Direccion de envio: <b>" + ship_address.ToString() + "</b><br />"
            html += "Fecha de apertura: <b>" + fecha.ToString() + "</b><br />"
            html += "Fecha requerido: <b>" + rsd.ToString() + "</b><br />"
            html += "Fecha de envio: <b>" + envio_fecha.ToString() + "</b><br /></h3>"
        Else
            html += "<b> No se tiene mas informacion del pedido</b>"
        End If

        Return html
    End Function



End Class
