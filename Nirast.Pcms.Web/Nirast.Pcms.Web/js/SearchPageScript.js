function checkRequiredBookingDetails() {
   
    if (($('#hdnUserType').val() != '' || $('#hdnUserType').val() != undefined) && $('#hdnUserType').val() != 'Caretaker' || $('#hdnUserType').val() != 'CLIENT') {
        if (localStorage.getItem("bookingFromDate") == '' || localStorage.getItem("bookingFromDate") == undefined || localStorage.getItem("bookingFromDate") == null) {
            alert("Please select the service required from date.");
            $('#FromDate').focus();
            return false;
        }

        if (localStorage.getItem("bookingFromTime") == '' || localStorage.getItem("bookingFromTime") == undefined || localStorage.getItem("bookingFromTime") == null) {
            alert("Please select the service required from time.");
            $('#FromTime').focus();
            return false;
        }

        if (localStorage.getItem("bookingToDate") == '' || localStorage.getItem("bookingToDate") == undefined || localStorage.getItem("bookingToDate") == null) {
            alert("Please select the service required to date.");
            $('#ToDate').focus();
            return false;
        }

        if (localStorage.getItem("bookingToTime") == '' || localStorage.getItem("bookingToTime") == undefined || localStorage.getItem("bookingToTime") == null) {
            alert("Please select the service required to time.");
            $('#PToTime').focus();
            return false;
        }
    }
    return true;
}

function checkRequiredId(id) {  
    debugger;
    var fromDate = $('#FromDate').val();
    var fromTime = $('#FromTime').val();
    var toDate = $('#ToDate').val();
    var toTime = $('#ToTime').val();
    var serviceId = $('#Services').val();

    if ($('#hdnLoggedUser').val() == '' || $('#hdnLoggedUser').val() == undefined) {
        alert('Please login first to book a service');
        e.preventDefault(e);
        return true;
    }

    if ($('#hdnUserType').val().toLowerCase() == 'caretaker' || $('#hdnUserType').val() == 'CLIENT') {
        alert("Booking can be done as Public User only.");
        //e.preventDefault(e);
        return true;
    }

    if (fromDate.length == 0 || fromTime.length == 0 || toDate.length == 0 || toTime.length == 0) {
        alert("Please provide service required date time details.");
        e.preventDefault(e);
        return true;

    }
    
    //var url = window.location.href;
    //var serviceid = url.substring(url.lastIndexOf("=") + 1, url.length);

    var src = '/CareTaker/CareTakerDetailPage/' + id + '?';
        src = src + "&dateFrom=" + fromDate
        src = src + "&dateTo=" + toDate
        src = src + "&timeFrom=" + fromTime
        src = src + "&timeTo=" + toTime
    src = src + "&serviceId=" + serviceId
    
    window.location.href = src
    return false;
}
function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}

function GetServiceDescription(service) {
   
    var seviceHeading = '';
    var serviceId = (service == null ||  service == undefined || service == NaN) ? 0 : service;
    $.ajax({
        url: "../Admin/GetServiceById/",
        data: { serviceId: serviceId },
        success: function (feed) {
            $('#ServiceDescriptions').html('');
            $.each($.parseJSON(feed), function (i, item) {
                //seviceHeading += '<div class = "sign-gd-two" h4 style="padding-top:30px;padding-bottom:10px;">' + item.Name + '</h4></div><div class=" row col-md-12 panel-body">' + item.ServiceDescription + '</div>'
                seviceHeading += '<div class="row"><div class="col-md-12"><div class="titleMain"><h4 style="padding-top:15px;padding-bottom:5px;">' + item.Name + '</h4></div></div></div><div class="row"><div class="col-md-12 panel-body" style="padding-top:10px;padding-bottom:15px;">' + item.ServiceDescription + '</div></div>'
            });
            $('#ServiceDescriptions').html(seviceHeading);
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while Searching a Caregiver. Function: $("#btnKeywordSearch").click(). Page: SearchPageScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}

$(document).ready(function () {

    $("#Result").on("click", "a.link-product-add-cart ", function (e) {
        var fromDate = $('#FromDate').val();
        var fromTime = $('#FromTime').val();
        var toDate = $('#ToDate').val();
        var toTime = $('#ToTime').val();
        if ($('#hdnLoggedUser').val() == '' || $('#hdnLoggedUser').val() == undefined) {
            alert('Please login first to book a service');
            e.preventDefault(e);
            return true;
        }

        if ($('#hdnUserType').val().toLowerCase() == 'caretaker' || $('#hdnUserType').val() == 'CLIENT') {
            alert("Booking can be done as Public User only.");
            //e.preventDefault(e);
            return true;
        }

        if (fromDate.length == 0 || fromTime.length == 0 || toDate.length == 0 || toTime.length == 0) {
            alert("Please provide service required date time details.");
            e.preventDefault(e);
            return true;

        }

        
        //var addressValue = $(this).attr("href");
        //var last = addressValue.substring(addressValue.lastIndexOf("/") + 1, addressValue.length);
        //alert(addressValue);
        //var Id = last;
        //$.ajax({
        //    url: "../CareTaker/CareTakerDetailPageWithRate",
        //    type: "POST",
        //    data: { caretakerId: last, dateFrom: fromDate, dateTo: toDate, timeFrom: fromTime, timeTo: toTime },
        //    success: function (data) {

        //    },
        //    error: function () {
        //        alert("Some network error has occurred. Please try again after some time.");
        //    }
        //});

    });

    $("#Result").on("click", "a.item_add", function (e) {
        
        var fromDate = $('#FromDate').val();
        var fromTime = $('#FromTime').val();
        var toDate = $('#ToDate').val();
        var toTime = $('#ToTime').val();

        if ($('#hdnLoggedUser').val() == '' || $('#hdnLoggedUser').val() == undefined) {
            alert('Please login first to book a service');
            e.preventDefault(e);
            return true;
        }

        if ($('#hdnUserType').val().toLowerCase() == 'caretaker' || $('#hdnUserType').val() == 'CLIENT') {
            alert("Booking can be done as Public User only.");
            e.preventDefault(e);
            return true;
        }

        if (fromDate.length == 0 || fromTime.length == 0 || toDate.length == 0 || toTime.length == 0) {
            alert("Please provide service required date time details.");
            e.preventDefault(e);
            return true;
           
        }

        if (!checkTime()) {
            event.preventDefault();
            return false;
        }

        
    });
    //$('#FromDate').focus();

    $('#searchBtn').click(function () {
        debugger;
        $('#displayError').hide();
        var validated = checkRequiredBookingDetails();
        //var serviceId = GetParameterValues('serviceId');
        //$('#Services').val(serviceId);
        $('#errorMsg').addClass('hidden');
        if (!validated) {
            event.preventDefault();
            return false;
        }
        if (!checkTime()) {
            event.preventDefault();
            return false;
        }
        var data = $('#myForm').serialize();
        showProgress();
        $.ajax({
            url: "../Home/SearchCareTaker",
            method: "post",
            data: data,
            success: function (feed) {
                $('#Result').html('');
                $('#Result').html(feed);
              
                spinnerVisible = true;
                hideProgress();
            },
            error: function (feed) {
                $('#errorMsg').removeClass('hidden');
                $('#Result').html('');
                $('#displayError').show();
             
                spinnerVisible = true;
                hideProgress();
                logError(feed.statusText + ' - Error occurred while Searching a Caregiver. Function: $("#searchBtn").click(). Page: SearchPageScript.js');
            }
        });
        $('#sortByRate').val('0');
        $('#sortByExp').val('0');
        closeAdvancedSearch();
    });

    $('#btnKeywordSearch').click(function () {
        var data = $('#txtKeyword').val();
        $.ajax({
            url: "../Home/KeywordCareTakerSearchDetail",
            data: { keyword: data },
            success: function (feed) {
                $('#Result').html('');
                $('#Result').html(feed);
            },
            error: function (data) {
                logError(data.statusText + ' - Error occurred while Searching a Caregiver. Function: $("#btnKeywordSearch").click(). Page: SearchPageScript.js');
                //alert('Some network error has occurred. Please try again after some time.');
            }
        });
        $('#advancedSearchMenu').hide();
    });
    var serviceId = GetParameterValues('serviceId');
   

    $('#Services').val(serviceId);
    GetServiceDescription(serviceId);
    $("#FromDate").val(localStorage.getItem("bookingFromDate"));
    $("#FromTime").val(localStorage.getItem("bookingFromTime"));
    $("#ToDate").val(localStorage.getItem("bookingToDate"));
    $("#ToTime").val(localStorage.getItem("bookingToTime"));
    $("#Country").val(localStorage.getItem("bookingCountry"));
    $("#State").val(localStorage.getItem("bookingState"));
    $("#City").val(localStorage.getItem("bookingCity"));
    $("#Category").val(localStorage.getItem("bookingType"));   
    $("#Gender").val(localStorage.getItem("bookingGender"));
    $("#Price").val(localStorage.getItem("bookingPrice"));
    $("#Experience").val(localStorage.getItem("bookingExp"));
    $("#ProfileId").val(localStorage.getItem("bookingProfile"));
    if (serviceId != undefined) {
        showProgress();
        debugger;
        if (localStorage.getItem("bookingFromDate") != undefined && localStorage.getItem("bookingFromDate") != ''
            && localStorage.getItem("bookingFromTime") != undefined && localStorage.getItem("bookingFromTime") != ''
            && localStorage.getItem("bookingToDate") != undefined && localStorage.getItem("bookingToDate") != ''
            && localStorage.getItem("bookingToTime") != undefined && localStorage.getItem("bookingToTime") != '') {

            $('#displayError').hide();
            var validated = checkRequiredBookingDetails();
            //var serviceId = GetParameterValues('serviceId');
            //$('#Services').val(serviceId);
            $('#errorMsg').addClass('hidden');
            if (!validated) {
                event.preventDefault();
                return false;
            }
            if (!checkTime()) {
                event.preventDefault();
                return false;
            }
            var data = $('#myForm').serialize();
            showProgress();
            $.ajax({
                url: "../Home/SearchCareTaker",
                method: "post",
                data: data,
                success: function (feed) {
                    $('#Result').html('');
                    $('#Result').html(feed);

                    spinnerVisible = true;
                    hideProgress();
                },
                error: function (feed) {
                    $('#errorMsg').removeClass('hidden');
                    $('#Result').html('');
                    $('#displayError').show();

                    spinnerVisible = true;
                    hideProgress();
                    logError(feed.statusText + ' - Error occurred while Searching a Caregiver. Function: $("#searchBtn").click(). Page: SearchPageScript.js');
                }
            });
            $('#sortByRate').val('0');
            $('#sortByExp').val('0');
            closeAdvancedSearch();
        }
        else {

            $.ajax({
                url: "../Home/SearchByService",
                data: { serviceId: serviceId },
                success: function (feed) {
                    $('#Result').html(feed);
                    spinnerVisible = true;
                    hideProgress();
                },
                error: function (feed) {
                    //alert("An error has occurred. " + feed);
                    spinnerVisible = true;
                    hideProgress();
                    logError(feed.statusText + ' - Error occurred while Searching a Caregiver for ServiceId:' + serviceId + '. Function: "../Home/SearchByService". Page: SearchPageScript.js');
                }
            });
        }
    }
});

$(function () {
    $("#FromTime").timepicker({ 'timeFormat': 'h:i A' });
    $("#ToTime").timepicker({ 'timeFormat': 'h:i A' });
});
    $(function () {
        $("#BookingStartTime").timepicker({ 'timeFormat': 'h:i A' });
        $("#BookingEndTime").timepicker({ 'timeFormat': 'h:i A' });
    });

$(document).ready(function () {
    $("#btnAddPlace").click(function () {

       // alert($('iframe[name=select_frame]').contents().find('#address').val())

        //var elementCollection = document.getElementsByClassName("title full-width");
        var locationName = null;
        //if (elementCollection.length > 0) {
        //    locationName = elementCollection[0].textContent;
        //}
        //if (locationName == null) {
        locationName = $('iframe[name=select_frame]').contents().find('#address').val();
        //}
        document.getElementById("Location").value = locationName;
        $('#searchBtn').click();
    });
})
var SelectedCountryId = function (selectObject) {
    var value = selectObject.value;
    $.ajax({
        //url: "http://pcms-api.nirasystems.com/api/Admin/GetStatesByCountryId/" + value,
        url: "http://localhost:8592/api/Admin/GetStatesByCountryId/" + value,
        success: function (data) {
            console.log(data);
            var ddlState = $("#State");
            ddlState.empty().append('<option selected="selected" value="0">--Select--</option>');
            var ddlCity = $("#City");
            ddlCity.empty().append('<option selected="selected" value="0">--Select--</option>');
            $.each(data, function (data, value) {
                ddlState.append($("<option></option>").val(value.StateId).html(value.Name));
            });
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + value +' . Function: var SelectedCountryId = function (selectObject). Page: SearchPageScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}

function SortData(sortBy, sortField) {
    debugger;
    if (sortField == 'Rate') {
        $('#sortByExp').val(0);
    } else {
        $('#sortByRate').val(0);
    }

    var data = $('#myForm').serialize();
    showProgress();
    $.ajax({
        url: "../Home/SearchCareTaker/",
        data: { searchInputs: data, sortField: sortField, sortBy: sortBy },
        method:"POST",
        success: function (feed) {
            $('#Result').html('');
            $('#Result').html(feed);
            spinnerVisible = true;
            hideProgress();
        }, error: function (feed) {
            spinnerVisible = true;
            hideProgress();
            logError(feed.statusText + ' - Error occurred while sorting caretakers. Function: SortData(sortBy, sortField). Page: SearchPageScript.js');
        }
    });
    $('#advancedSearchMenu').hide();
}

var SelectedStateId = function (selectObject) {
    var value = selectObject.value;
    $.ajax({
        //url: "http://pcms-api.nirasystems.com/api/City/GetCityByStateId/" + value,
        url: "http://localhost:8592/api/City/GetCityByStateId/" + value,
        success: function (data) {
            console.log(data);
            var ddlCity = $("#City");
            ddlCity.empty().append('<option selected="selected" value="0">--Select--</option>');
            $.each(data, function (data, value) {
                ddlCity.append($("<option></option>").val(value.CityId).html(value.CityName));
            });
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + value +' . Function: var SelectedCountryId = function (selectObject). Page: SearchPageScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}

$(function () {
    $('#dtpPFromDate').datepicker({ 'dateFormat': 'dd-MM-yy' });
    $('#dtpPToDate').datepicker({ 'dateFormat': 'dd-MM-yy' });
    $('#FromDate').datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: 0,
        'dateFormat': 'dd-MM-yy'
    });
    $('#ToDate').datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: 0,
        'dateFormat': 'dd-MM-yy'
    });
});


// This example adds a search box to a map, using the Google Place Autocomplete
// feature. People can enter geographical searches. The search box will return a
// pick list containing a mix of places and predicted search terms.

// This example requires the Places library. Include the libraries=places
// parameter when you first load the API. For example:
// <script src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&libraries=places">
var infoWindow;
function initAutocomplete() {
    var map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -33.8688, lng: 151.2195 },
        zoom: 13,
        mapTypeId: 'roadmap'
    });


    // Create the search box and link it to the UI element.
    var input = document.getElementById('pac-input1');
    $(input).on('input', function () {

        $('.pac-container').attr('z-index', 1050);
    });
    var searchBox = new google.maps.places.SearchBox(input);
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    // Bias the SearchBox results towards current map's viewport.
    map.addListener('bounds_changed', function () {

        searchBox.setBounds(map.getBounds());
    });

    var markers = [];
    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    searchBox.addListener('places_changed', function () {

        var places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        markers.forEach(function (marker) {
            marker.setMap(null);
        });
        markers = [];

        // For each place, get the icon, name and location.
        var bounds = new google.maps.LatLngBounds();
        places.forEach(function (place) {
            if (!place.geometry) {
                console.log("Returned place contains no geometry");
                return;
            }
            var icon = {
                url: place.icon,
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25)
            };

            // Create a marker for each place.
            markers.push(new google.maps.Marker({
                map: map,
                icon: icon,
                title: place.name,
                position: place.geometry.location
            }));

            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
        });
        map.fitBounds(bounds);
    });
    infoWindow = new google.maps.InfoWindow;

    // Try HTML5 geolocation.
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            infoWindow.setPosition(pos);
            infoWindow.setContent('Your Location.');
            infoWindow.open(map);
            map.setCenter(pos);
        }, function () {
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());
    }
}
function handleLocationError(browserHasGeolocation, infoWindow, pos) {
    infoWindow.setPosition(pos);
    infoWindow.setContent(browserHasGeolocation ?
        'Error: The Geolocation service failed.' :
        'Error: Your browser doesn\'t support geolocation.');
    infoWindow.open(map);
}

function closeAdvancedSearch() {
    $('#advancedSearch').removeClass('in');
    $('h4.panel-title').addClass('collapsed');
    $(document).find(".glyphicon").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
}

$(document).ready(function () {
    // Add minus icon for collapse element which is open by default
    $(".collapse.in").each(function () {
        $(this).siblings(".panel-heading").find(".glyphicon").addClass("glyphicon-chevron-up").removeClass("glyphicon-chevron-down");
    });

    // Toggle plus minus icon on show hide of collapse element
    $(".collapse").on('show.bs.collapse', function () {
        $(this).parent().find(".glyphicon").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
    }).on('hide.bs.collapse', function () {
        $(this).parent().find(".glyphicon").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
    });

    //localStorage.setItem("bookingFromDate", '');
    //localStorage.setItem("bookingFromTime", '');
    //localStorage.setItem("bookingToDate", '');
    //localStorage.setItem("bookingToTime", '');

});

//function LoadStates(selectedCountry) {
//    $.ajax({
//        url: '/Admin/LoadStatesByCountryId',
//        type: "GET",
//        dataType: "JSON",
//        data: { countryId: selectedCountry },
//        success: function (data) {

//            $("#State").html(""); // clear before appending new list
//            $("#State").append(
//                $('<option></option>').val(0).html("--State/Province--"));
//            $.each($.parseJSON(data), function (i, state) {

//                $("#State").append(
//                    $('<option></option>').val(state.StateId).html(state.Name));
//            });
//            $('#City').html('');
//            $('#City').append($('<option></option>').val('0').html('--City--'));
//        },
//    });
//}

function LoadCities(selectedState) {
    $.ajax({
        url: '/Admin/LoadCitiesbyStateId',
        type: "GET",
        dataType: "JSON",
        data: { StateId: selectedState },
        success: function (data) {
            $("#CityId").html(""); // clear before appending new list
            $("#CityId").append(
                $('<option></option>').val("").html("--Select--"));
            $.each($.parseJSON(data), function (i, city) {
                $("#CityId").append(
                    $('<option></option>').val(city.CityId).html(city.CityName));
            });
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading Cities for StateId: ' + selectedState +' . Function: LoadCities(selectedState). Page: SearchPageScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
    $("#CityId").prop('selectedIndex', 0);

}

function resetSearch() {
    //window.location.reload();
    $('#FromDate').val('');
    $('#FromTime').val('');
    $('#ToDate').val('');
    $('#ToTime').val('');
    $('#Location').val('');
    $('#ProfileId').val('');
    $('#Category').prop('selectedIndex', 0);
    $('#Price').prop('selectedIndex', 0);
    $('#Experience').prop('selectedIndex', 0);
    $('#Services').prop('selectedIndex', 0);
    $('#Gender').prop('selectedIndex', 0);
    $('#Country').prop('selectedIndex', 1);
    //$('#State').prop('selectedIndex', 0);
    LoadStatesByCountry(1);
    $('#City').html('');
    $('#City').append($('<option></option>').val('0').html('--Select--'));
    localStorage.setItem("bookingFromDate", '');
    localStorage.setItem("bookingFromTime", '');
    localStorage.setItem("bookingToDate", '');
    localStorage.setItem("bookingToTime", '');
    localStorage.setItem("bookingCountry", '');
    localStorage.setItem("bookingState", '');
    localStorage.setItem("bookingCity", '');
    localStorage.setItem("bookingType", '');
    localStorage.setItem("bookingService", '');
    localStorage.setItem("bookingGender", '');
    localStorage.setItem("bookingPrice", '');
    localStorage.setItem("bookingExp", '');
    localStorage.setItem("bookingProfile", '');
    
}

function checkTime() {
    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();
    var stt = new Date($('#FromDate').val() + ' ' + $('#FromTime').val());
    var endt = new Date($('#ToDate').val() + ' ' + $('#ToTime').val());
    var now = new Date();
    if (Date.parse(now.toDateString()) == Date.parse(fromDate)) {
        if (stt.getTime() <= now.getTime() && $('#FromTime').val() != '') {
            alert('From time should be greater than current time.');
            $('#FromTime').val('');
            $('ul.ui-timepicker-list li').removeClass('ui-timepicker-selected');
            $('.ui-timepicker-wrapper').css("display", "none");
            return false;
        }

        if (endt.getTime() <= now.getTime() && $('#ToTime').val() != '') {
            alert('To Time should be greater than Current Time.');
            $('#ToTime').val('');
            $('ul.ui-timepicker-list li').removeClass('ui-timepicker-selected');
            $('.ui-timepicker-wrapper').css("display", "none");
            return false;
        }
    }
    if (fromDate == toDate && $('#FromDate').val() != '' && $('#FromTime').val() != '' && $('#ToDate').val() != '' && $('#ToTime').val() != '') {
        stt = stt.getTime();
        endt = endt.getTime();
        if (stt >= endt) {
            $('#ToTime').val('');
            $('ul.ui-timepicker-list li').removeClass('ui-timepicker-selected');
            $('.ui-timepicker-wrapper').css("display", "none");
            localStorage.setItem("bookingToTime", '');
            alert('Service From Time should be greater than To Time');
            return false;
        }
    }
    return true;
}
function setBookingFromDate(value) {
    if (checkTime()) {
        var fromDate = $('#FromDate').val();
        $('#ToDate').val(fromDate);
        var toDate = $('#ToDate').val();
        if (fromDate != "") {
            if (toDate != "") {
                if (Date.parse(fromDate) > Date.parse(toDate)) {
                    alert("To date cannot be less than From date.");
                    
                    return false;
                }
            }
        }
    }
    $('#ToDate').val(fromDate);
    localStorage.setItem("bookingToDate", value);
    localStorage.setItem("bookingFromDate", value);
}

function setBookingFromTime(value) {
    if (checkTime()) {
        localStorage.setItem("bookingFromTime", value);
    }
}

function setBookingToDate(value) {
    if (checkTime()) {
        var fromDate = $('#FromDate').val();
        var toDate = $('#ToDate').val();
        if (fromDate != "") {
            if (toDate != "") {
                if (Date.parse(fromDate) > Date.parse(toDate)) {
                    alert("To date cannot be less than From date.");
                    $('#ToDate').val(fromDate);
                    localStorage.setItem("bookingToDate", fromDate);
                    return false;
                }
            }
        }
    }
    localStorage.setItem("bookingToDate", value);
}

function setBookingToTime(value) {
    checkTime();
    localStorage.setItem("bookingToTime", value);
}
function dateRangeValidation(value) {
   
    var fromDate = $('#txtFromDate').val();
    var toDate = $('#txtToDate').val();
        if (fromDate != "") {
            if (toDate != "") {
                if (Date.parse(fromDate) > Date.parse(toDate)) {
                    alert("To date cannot be less than From date.");
                  
                    return false;
                }
            }
        
    }
  
}