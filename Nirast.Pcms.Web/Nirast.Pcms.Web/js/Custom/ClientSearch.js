$(document).ready(function () {


    $('#caretakerList').DataTable({
        "pagingType": "full_numbers"
    });
   
    $('.filters').hide();
    //$('#horizontalTab').easyResponsiveTabs({
    //    type: 'default', //Types: default, vertical, accordion
    //    width: 'auto', //auto or any width like 600px
    //    fit: true   // 100% fit in a container
    //});

    $(".photo#website2").click(function () {
        var location = 'Wasington DC';
        document.getElementById("txtMapLocation").value = location;

    });

    $('.filterable .btn-filter').click(function () {
        var $panel = $(this).parents('.filterable'),
            $filters = $panel.find('.filters input'),
            $tbody = $panel.find('.table tbody');
        if ($filters.prop('disabled') == true) {
            $filters.prop('disabled', false);
            $('.filters').show();
            $('.non-filter-header').hide();
            $filters.first().focus();
        } else {
            $filters.val('').prop('disabled', true);
            $tbody.find('.no-result').remove();
            $tbody.find('tr').show();
            $('.non-filter-header').show();
            $('.filters').hide();
        }
    });

    $('.filterable .filters input').keyup(function (e) {
        /* Ignore tab key */
        var code = e.keyCode || e.which;
        if (code == '9') return;
        /* Useful DOM data and selectors */
        var $input = $(this),
            inputContent = $input.val().toLowerCase(),
            $panel = $input.parents('.filterable'),
            column = $panel.find('.filters th').index($input.parents('th')),
            $table = $panel.find('.table'),
            $rows = $table.find('tbody tr');
        /* Dirtiest filter function ever ;) */
        var $filteredRows = $rows.filter(function () {
            var value = $(this).find('td').eq(column).text().toLowerCase();
            return value.indexOf(inputContent) === -1;
        });
        /* Clean previous no-result if exist */
        $table.find('tbody .no-result').remove();
        /* Show all rows, hide filtered ones (never do that outside of a demo ! xD) */
        $rows.show();
        $filteredRows.hide();
        /* Prepend no-result row if all rows are filtered */
        if ($filteredRows.length === $rows.length) {
            $table.find('tbody').prepend($('<tr class="no-result text-center"><td colspan="' + $table.find('.filters th').length + '">No result found</td></tr>'));
        }
    });
    $('#searchBtn').click(function () {
        $.ajax({
            url: "../Client/ClientSearch",
            type: "POST",
            data: { clientName: $('#ClientName').val(), location: $('#Location').val() },
            success: function (data) {
                $("body").html(data);
            },
            error: function (data) {
                logError(status + ' - Error occurred while searching the Client. Function: $("#searchBtn").click(function ()). Page: ClientSearch.js');
                //alert('Some network error has occurred. Please try again after some time.');
            }
        });
    });

});