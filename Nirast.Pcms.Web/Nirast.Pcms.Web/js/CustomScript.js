
function LoadStates() {

    var selectedCountry = ($('#CountryId').val() === "" || $('#CountryId').val() === undefined) ? 0 : $('#CountryId').val();
    $.ajax({
        url: '/Admin/LoadStatesByCountryId',
        type: "GET",
        dataType: "JSON",
        data: { countryId: selectedCountry },
        success: function (data) {

            $("#StateId").html(""); // clear before appending new list
            $("#StateId").append(
                $('<option></option>').val("").html("--Select--"));
            if (selectedCountry !==0) {
                $.each($.parseJSON(data), function (i, state) {

                    $("#StateId").append(
                        $('<option></option>').val(state.StateId).html(state.Name));
                });
            }
            $("#StateId").prop('selectedIndex', 0);
            $('#CityId').html('');
            $('#CityId').append($('<option></option>').val('').html('--Select--'));
            $.ajax({
                url: '/admin/loadphonecodebycountryid',
                type: "get",
                datatype: "json",
                data: { countryid: selectedCountry },
                success: function (data) {
                    $('#lblPhoneCodePrimary1').text(data);
                    $('#lblPhoneCodePrimary2').text(data);

                },
                error: function (data) {
                    logError(data.statusText + ' - Error occurred while getting Phone Code for CountryId:' + selectedCountry + ' in function LoadStates(). Page: CustomScript.js');
                    //alert('Some network error has occurred. Please try again after some time.');
                }
            });
        },
    });
}
function SignUp() {
    $('.spinner').css('display', 'block');
}
function editServiceDetails(serviceId, description, name,ServiceOrder, ServicePicture) {
    $('#Name').val(name);
    $('#ServiceDescription').val(description);
    $('#ServiceOrder').val(ServiceOrder);
    $('#ServiceId').val(serviceId);
    $('#ServicePicture').val(ServicePicture);
}

function editDesignationDetails(designationId, designation) {
    $('#Designation').val(designation);
    $('#DesignationId').val(designationId);
}
function editQualificationDetails(qualificationId, qualification) {
    $('#Qualification').val(qualification);
    $('#QualificationId').val(qualificationId);
}
function editRoleDetails(roleId, roleName) {
    $('#RoleId').val(roleId);
    $('#RoleName').val(roleName);
}
function editQuestionDetails(questionId, question, sortOrder) {
    $('#Questions').val(question);
    $('#SortOrder').val(sortOrder);
    $('#QuestionId').val(questionId);
}
function editCategoryDetails(categoryId, name, color) {
    $('#CategoryId').val(categoryId);
    $('#Name').val(name);
    $('#ddlColor').val(color);
    colorbtn.style.background = color;
}
function editConfigDetails(configId,configName,mailHost,mailPort,ssl) {
    $('#ConfigId').val(configId);
    $('#MailHost').val(mailHost);
    $('#ConfigName').val(configName);
    $('#MailPort').val(mailPort);
    $('#IsSSL').prop("checked", ssl);
}
function editEmailTypeDetails(configId, emailType, fromEmail, password) {
    $('#ddBranch').val(BranchId);
    $('#ConfigId').val(configId);
    $('#FromEmail').val(fromEmail);
    $('#EmailtypeId').val(emailType);
    $('#Password').val(password);
}
function editCountryDetails(countryId, code, name, phoneCode, currency, currencySymbol, isDefault) {
    $('#Code').val(code);
    $('#Name').val(name);
    $('#CountryId').val(countryId);
    $('#PhoneCode').val(phoneCode);
    $('#Currency').val(currency);
    $('#CurrencySymbol').val(currencySymbol);
    $('#IsDefault').prop("checked", isDefault);
}

function editWorkShiftDetails(shiftId, shiftName, showResidentName, IsInvoiceSeparately) {
    $('#ShiftName').val(shiftName);
    $('#ShiftId').val(shiftId);
    $('#ShowResidentName').prop("checked", showResidentName == 'true' ? true : false);
    $('#IsSeparateInvoice').prop("checked", IsInvoiceSeparately == 'true' ? true : false);
}
/*PAYMENT ADVANCED SEARCH*/
$('#paymentSearchbtn').click(function () {
    var month = $("#ddlMonthly").val();
    var fromdate = $("#txtFromDate").val();
    var todate = $("#txtToDate").val();
    var dateRange = $("#ddlSearchRange").val();
    if (dateRange == 2 && month == 0) {
        alert("Please select any month.");
        return;
    }
    else if (dateRange == 3) {
        if (fromdate == '') {
            alert('Select from date.');
            return false;
        }
        if (todate == '') {
            alert('Select to date.');
            return false;
        }
    }
    showProgress();
    var data = $('#searchForm').serialize();
    $.ajax({
        url: "../Admin/SearchPaymentHistory",
        data: data,
        success: function (data) {
            spinnerVisible = true;
            hideProgress();
            $("#History").html('');
            $("#History").html(data);
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while Searching payment history. Function: $("#paymentSearchbtn").click(). Page: CustomScript.js', '');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
    // closeAdvancedSearch();
});

function resetPaymentSearch() {
    $('#txtFromDate').val('');
    $('#txtToDate').val('');
    $('#CareTakerId').val('');
    $('#ddlSearchRange').prop('selectedIndex', 0);
    $('#ddlYearly').prop('selectedIndex', 0);
    $('#ddlMonthly').prop('selectedIndex', 0);
    $('#Service').prop('selectedIndex', 0);
    $('#Category').prop('selectedIndex', 0);

}
/*****************/
function editTimeShiftDetails(timeShiftId, timeShiftName, workingHours, payingHours, startTime) {
    $('#TimeShiftName').val(timeShiftName);
    $('#WorkingHours').val(workingHours);
    $('#PayingHours').val(payingHours);
    $('#StartTime').val(startTime);
    $('#TimeShiftId').val(timeShiftId);
    $('#StartTime').val(startTime);
    $('#PayHours').val(payHours);
}

function deleteClientTimeShiftDetail(timeShiftid) {

}
function editHolidayDetails(countryId, stateId, holidayId, holiday, holidayDate) {
    $('#CountryId').val(countryId);
    LoadStatesforHoliday(countryId, stateId);

    $('#HolidayName').val(holiday);
    $('#HolidayDate').val(holidayDate.toString("mm/dd/yyyy"));
    $('#HolidayId').val(holidayId);
}


function LoadStatesforHoliday(selectedCountry, stateId) {
    $.ajax({
        url: '/Admin/LoadStatesByCountryId',
        type: "GET",
        dataType: "JSON",
        data: { countryId: selectedCountry },
        success: function (data) {
            $("#StateId").html(""); // clear before appending new list
            $("#StateId").append(
                $('<option></option>').val(0).html("--Select--"));
            $.each($.parseJSON(data), function (i, state) {

                $("#StateId").append(
                    $('<option></option>').val(state.StateId).html(state.Name));
            });
            $('#StateId').val(stateId);
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + selectedCountry + '. Function: LoadStatesforHoliday(selectedCountry, stateId). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}




function SelectedCountryId(selectedIndex) {
    $('#CountryId').val(selectedIndex);
}

function SelectedStateId(selectedIndex) {
    $('#StateId').val(selectedIndex);
}

function GetStates(StateId = 0, CityId = 0) {
    var countryId = ($('#ddlCountry').val() === "" || $('#ddlCountry').val() === undefined) ? 0 : $('#ddlCountry').val();
    $('#CountryId').val(countryId);
    $.ajax({
        url: '/Admin/LoadStatesByCountryId',
        type: "GET",
        dataType: "JSON",
        data: { CountryId: countryId },
        success: function (data) {
            $("#ddlStates").html(""); // clear before appending new list

            $("#ddlStates").append(
                $('<option></option>').val("").html("--Select--"));
            $("#ddlCity").html("");
            $("#ddlCity").append(
                $('<option></option>').val("").html("--Select--"));
            if (countryId !== 0) {
                $.each($.parseJSON(data), function (i, state) {
                    $("#ddlStates").append(
                        $('<option></option>').val(state.StateId).html(state.Name));
                });

                document.getElementById("ddlStates").value = StateId;
            }
            $("#ddlStates").prop('selectedIndex', 0);
            if (CityId !== 0) {
                GetCities(CityId);
            }
            $.ajax({
                url: '/admin/loadphonecodebycountryid',
                type: "get",
                datatype: "json",
                data: { countryid: countryId },
                success: function (data) {
                    $('#lblPhoneCodePrimary1').text(data);
                    $('#lblPhoneCodePrimary2').text(data);

                },
                error: function (data) {
                    logError(data.statusText + ' - Error occurred while loading phone code for CountryId: ' + countryId + '. Function: GetStates(StateId = 0, CityId = 0). Page: CustomScript.js');
                    //alert('Some network error has occurred. Please try again after some time.');
                }
            });
            //$('#lblPhoneCodePrimary').text($.parseJSON(data)[0] === undefined ? '+1' : $.parseJSON(data)[0].PhoneCode);
            //$('#lblPhoneCodeSecondary').text($.parseJSON(data)[0] === undefined ? '+1' : $.parseJSON(data)[0].PhoneCode);
            //if ($("#lblPhoneCodePrimary").length) {
            //    $('#lblPhoneCodePrimary').text($.parseJSON(data)[0] == undefined ? '+1' : $.parseJSON(data)[0].PhoneCode);
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + countryId + '. Function: GetStates(StateId = 0, CityId = 0). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}

function GetStates2(StateId = 0, CityId = 0) {
   
    var countryId = ($('#ddlCountry').val() === "" || $('#ddlCountry').val() === undefined) ? 0 : $('#ddlCountry').val();
    $('#CountryId2').val(countryId);
    $.ajax({
        url: '/Admin/LoadStatesByCountryId',
        type: "GET",
        dataType: "JSON",
        data: { CountryId: countryId },
        success: function (data) {

            $("#ddlStates").html(""); // clear before appending new list
            $("#ddlStates").append(
                $('<option></option>').val("").html("--Select--"));
            $("#ddlCity").html(""); // clear before appending new list
            $("#ddlCity").append(
                $('<option></option>').val("").html("--Select--"));
            if (countryId != 0) {
                $.each($.parseJSON(data), function (i, state) {

                    $("#ddlStates").append(
                        $('<option></option>').val(state.StateId).html(state.Name));
                });
                document.getElementById("ddlStates").value = StateId;
            }
            $("#ddlStates").prop('selectedIndex', 0);
            if (CityId !== 0) {
                GetCities2(CityId);
            }
            $.ajax({
                url: '/admin/loadphonecodebycountryid',
                type: "get",
                datatype: "json",
                data: { countryid: countryId },
                success: function (data) {
                    $('#lblPhoneCodeSecondary1').text(data);
                    $('#lblPhoneCodeSecondary2').text(data);
                    $('.lblPhoneCodeSecondary1').text(data);
                    $('.lblPhoneCodeSecondary2').text(data);
                },
                error: function (data) {
                    logError(data.statusText + ' - Error occurred while loading phone code for CountryId: ' + countryId + '. Function: GetStates2(StateId = 0). Page: CustomScript.js');
                    //alert('Some network error has occurred. Please try again after some time.');
                }
            });
            //$('#lblPhoneCodePrimary').text($.parseJSON(data)[0] === undefined ? '+1' : $.parseJSON(data)[0].PhoneCode);
            //$('#lblPhoneCodeSecondary').text($.parseJSON(data)[0] === undefined ? '+1' : $.parseJSON(data)[0].PhoneCode);
            //if ($("#lblPhoneCodePrimary").length) {
            //    $('#lblPhoneCodePrimary').text($.parseJSON(data)[0] == undefined ? '+1' : $.parseJSON(data)[0].PhoneCode);
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + countryId + '. Function: GetStates2(StateId = 0).  Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}
function GetStates1(StateId = 0, CityId) {
  
    var countryId = ($('#ddlCountry1').val() === "" || $('#ddlCountry1').val() === undefined) ? 0 : $('#ddlCountry1').val();
    $('#CountryId1').val(countryId);
    $.ajax({
        url: '/Admin/LoadStatesByCountryId',
        type: "GET",
        dataType: "JSON",
        data: { CountryId: countryId },
        success: function (data) {

            $("#ddlStates1").html(""); // clear before appending new list
            $("#ddlStates1").append(
                $('<option></option>').val("").html("--Select--"));
           
            $("#ddlCity1").html("");
            $("#ddlCity1").append(
                $('<option></option>').val("").html("--Select--"));
            if (countryId !== 0) {
                $.each($.parseJSON(data), function (i, state) {

                    $("#ddlStates1").append(
                        $('<option></option>').val(state.StateId).html(state.Name));

                  
                });


                document.getElementById("ddlStates1").value = StateId;
            }
            $("#ddlStates").prop('selectedIndex', 0);
            GetCities1(CityId);
            $.ajax({
                url: '/admin/loadphonecodebycountryid',
                type: "get",
                datatype: "json",
                data: { countryid: countryId },
                success: function (data) {
                    $('#lblPhoneCodePrimary1').text(data);
                    $('#lblPhoneCodePrimary2').text(data);
                    $('.lblPhoneCodePrimary1').text(data);
                    $('.lblPhoneCodePrimary2').text(data);

                },
                error: function (data) {
                    logError(data.statusText + ' - Error occurred while loading phone code for CountryId: ' + countryId + '. Function: GetStates1(StateId = 0). Page: CustomScript.js.');
                    //alert('Some network error has occurred. Please try again after some time.');
                }
            });
            //if (CityId !== 0) {
            //    GetCities1(CityId);
            //}
          
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + countryId + '. Function: GetStates1(StateId = 0). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}
    function editCity(CityId, CountryId, StateId, CityName) {
    document.getElementById("ddlCountry").value = CountryId;
    //GetStates(StateId);
    document.getElementById("ddlStates").value = StateId;
    $('#CountryId').val(CountryId);
    $('#StateId').val(StateId);
    $('#CityId').val(CityId);
    $('#CityName').val(CityName);
}
function editResidentDetails(ResidentId,Name,ClientId,OtherInfo) {
  
    $('#ResidentId').val(ResidentId);
    $('#ResidentName').val(Name);
    $('#ClientId').val(ClientId);
    $('#OtherInfo').val(OtherInfo);
}
function LoadStatesByCountry(selectedCountry, stateId) {
    $.ajax({
        url: '/Admin/LoadStatesByCountryId',
        type: "GET",
        dataType: "JSON",
        data: { countryId: selectedCountry },
        success: function (data) {
            $("#ddlStates").html(""); // clear before appending new list
            $("#ddlStates").append(
                $('<option></option>').val(0).html("--Select--"));
            $.each($.parseJSON(data), function (i, state) {

                $("#ddlStates").append(
                    $('<option></option>').val(state.StateId).html(state.Name));
            });
            $('#ddlStates').val(stateId);
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + selectedCountry + '. Function: LoadStatesforHoliday(selectedCountry, stateId). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}
function LoadCityByStates(stateId, cityId) {
    $.ajax({
        url: '/Admin/LoadCitiesbyStateId',
        type: "GET",
        dataType: "JSON",
        data: { stateId: stateId },
        success: function (data) {
            $("#ddlCity").html(""); // clear before appending new list
            $("#ddlCity").append(
                $('<option></option>').val(0).html("--Select--"));
            $.each($.parseJSON(data), function (i, city) {

                $("#ddlCity").append(
                    $('<option></option>').val(city.CityId).html(city.CityName));
            });
            $('#ddlCity').val(cityId);
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading states for CountryId: ' + selectedCountry + '. Function: LoadStatesforHoliday(selectedCountry, stateId). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}

function editInvoiceDetails(ClientId, ClientName, InvoicePrefix, InvoiceNumber) {
    $('#ClientId').val(ClientId);
    $('#ddlClient').val(ClientId);
    $('#ClientName').val(ClientName);
    $('#InvoicePrefix').val(InvoicePrefix);
    $('#InvoiceNumber').val(InvoiceNumber);
}

function editUserInvoiceDetails(InvoicePrefix, InvoiceNumber) {
    debugger;
    $('#InvoicePrefix').val(InvoicePrefix);
    $('#InvoiceNumber').val(InvoiceNumber);
}
function GetCities(CityId = 0) {
   
    var StateId = ($('#ddlStates').val() === "" || $('#ddlStates').val() === undefined) ? 0 : $('#ddlStates').val();
    $('#StateId').val() !== undefined && StateId !== 0 ? $('#StateId').val(StateId) : $('#StateId').val('');
    $.ajax({
        url: '/Admin/LoadCitiesbyId',
        type: "GET",
        dataType: "JSON",
        data: { StateId: StateId },
        success: function (data) {
            $("#ddlCity").html(""); // clear before appending new list
            $("#ddlCity").append(
                $('<option></option>').val("").html("--Select--"));
            $.each($.parseJSON(data), function (i, city) {
                $("#ddlCity").append(
                    $('<option></option>').val(city.CityId).html(city.CityName));
            });
            $("#ddlCity").prop('selectedIndex', 0);
            if (CityId !== 0) {
                $("#ddlCity").val(CityId);
            }
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading Cities for StateId: ' + StateId + '. Function: GetCities(CityId = 0). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
}
function GetCities2(CityId = 0) {
    var StateId =($('#ddlStates').val() === "" || $('#ddlStates').val() === undefined) ? 0 : $('#ddlStates').val();
    $.ajax({
        url: '/Admin/LoadCitiesbyId',
        type: "GET",
        dataType: "JSON",
        data: { StateId: StateId },
        success: function (data) {
            $("#ddlCity").html(""); // clear before appending new list
            $("#ddlCity").append(
                $('<option></option>').val("").html("--Select--"));
            $.each($.parseJSON(data), function (i, city) {
                $("#ddlCity").append(
                    $('<option></option>').val(city.CityId).html(city.CityName));
            });
            $("#ddlCity").prop('selectedIndex', 0);
            if (CityId !== 0) {
                $("#ddlCity").val(CityId);
            }
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading Cities for StateId: ' + StateId + '. Function: GetCities2(). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
    $('#StateId2').val(StateId);
}


function GetCities1(CityId = 0) {
   
    var StateId = ($('#ddlStates1').val() === "" || $('#ddlStates1').val() === undefined) ? 0 : $('#ddlStates1').val();
    $.ajax({
        url: '/Admin/LoadCitiesbyId',
        type: "GET",
        dataType: "JSON",
        data: { StateId: StateId },
        success: function (data) {
            $("#ddlCity1").html(""); // clear before appending new list
            $("#ddlCity1").append(
                $('<option></option>').val("").html("--Select--"));
            $.each($.parseJSON(data), function (i, city) {
                $("#ddlCity1").append(
                    $('<option></option>').val(city.CityId).html(city.CityName));
            });
            $("#ddlCity").prop('selectedIndex', 0);
            if (CityId !== 0) {
                $("#ddlCity1").val(CityId);
            }
            //if ($('#CityId1').val() !== undefined)
            //    document.getElementById("ddlCity1").value = $('#CityId1').val();
            //else
            //    $("#ddlCity1 option:contains('--Select--')").attr('selected', 'selected');
        },
        error: function (data) {
            logError(data.statusText + ' - Error occurred while loading Cities for StateId: ' + StateId + '. Function: GetCities1(). Page: CustomScript.js');
            //alert('Some network error has occurred. Please try again after some time.');
        }
    });
    $('#StateId1').val(StateId);
}

function SetCity1() {
    $('#CityId1').val($('#ddlCity1').val());
}

function SetCity2() {
    $('#CityId2').val($('#ddlCity').val());
}

function storePreferredDateTime() {

    window.localStorage.setItem('PreferredFDate', document.getElementById('dtpPFromDate').value);
    window.localStorage.setItem('PreferredFTime', document.getElementById('PFromTime').value);
    window.localStorage.setItem('PreferredTDate', document.getElementById('dtpPToDate').value);
    window.localStorage.setItem('PreferredTTime', document.getElementById('PToTime').value);
    $('#advancedSearchMenu #FromDate').val($('#dtpPFromDate').val());
    $('#advancedSearchMenu #ToDate').val($('#dtpPToDate').val());

    $('#searchBtn').click();
}

function clearPreferredDateDetails() {
    window.localStorage.setItem('PreferredFDate', '');
    window.localStorage.setItem('PreferredFTime', '');
    window.localStorage.setItem('PreferredTDate', '');
    window.localStorage.setItem('PreferredTTime', '');
}

function GetCardType(number) {
    // visa
    var re = new RegExp("^4");
    if (number.match(re) != null)
        return "Visa";

    // Mastercard 
    // Updated for Mastercard 2017 BINs expansion
    if (/^(5[1-5][0-9]{14}|2(22[1-9][0-9]{12}|2[3-9][0-9]{13}|[3-6][0-9]{14}|7[0-1][0-9]{13}|720[0-9]{12}))$/.test(number))
        return "Mastercard";

    // AMEX
    re = new RegExp("^3[47]");
    if (number.match(re) != null)
        return "AMEX";

    // Discover
    re = new RegExp("^(6011|622(12[6-9]|1[3-9][0-9]|[2-8][0-9]{2}|9[0-1][0-9]|92[0-5]|64[4-9])|65)");
    if (number.match(re) != null)
        return "Discover";

    // Diners
    re = new RegExp("^36");
    if (number.match(re) != null)
        return "Diners";

    // Diners - Carte Blanche
    re = new RegExp("^30[0-5]");
    if (number.match(re) != null)
        return "Diners - Carte Blanche";

    // JCB
    re = new RegExp("^35(2[89]|[3-8][0-9])");
    if (number.match(re) != null)
        return "JCB";

    // Visa Electron
    re = new RegExp("^(4026|417500|4508|4844|491(3|7))");
    if (number.match(re) != null)
        return "Visa Electron";

    return "";
}

function checkLoginName() {
    debugger;
    $("#result").html("");
    email_regex = /^[a-zA-Z][a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i;
    var name = $('#EmailAddress').val();
    if (email_regex.test(name)) {
        if (name != "") {
            var url = "/PublicUser/CheckLoginNameExist";

            $.get(url, { loginName: name }, function (data) {
                if (data == "Y") {
                    $("#result").html("<span style='color:green'>Available</span>");
                    $("#txtEmailAddress").css('background-color', '');
                }
                else {

                    $("#result").html("<span style='color:#e97878'>Not Available</span>");
                    $("#txtEmailAddress").css('border-color', '#e97878');
                    $("#txtEmailAddress").css('border-style', 'solid !important');
                    $("#EmailAddress").focus();
                }
            });
        }
    }
}

function checkClientLoginName() {
    debugger;
    $("#result").html("");
    email_regex = /^[a-zA-Z][a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i;
    var name = $('#EmailId').val();
    if (email_regex.test(name)) {
        if (name != "") {
            var url = "/PublicUser/CheckLoginNameExist";

            $.get(url, { loginName: name }, function (data) {
                if (data == "Y") {
                    $("#result").html("<span style='color:green'>Available</span>");
                    $("#txtEmailAddress").css('background-color', '');
                }
                else {
                    var name = $('#result').val();
                    if (name == "") {

                        $("#result").html("<span style='color:#e97878'>Not Available</span>");
                    }
                    $("#txtEmailAddress").css('border-color', '#e97878');
                    $("#txtEmailAddress").css('border-style', 'solid !important');
                    $("#EmailId").focus();
                }
            });
        }
    }
}

function reload() {
    window.location.reload();
}

$body = $("body");

$(document).on({
    ajaxStart: function () { $body.addClass("loading"); },
    ajaxStop: function () { $body.removeClass("loading"); }
});

//*********************Start************************
// Function for showing and hiding Loading animation
var spinnerVisible = false;
function showProgress() {

    if (!spinnerVisible) {
        $("div#spinner").fadeIn("fast");
        spinnerVisible = true;
    }
};
function hideProgress() {

    if (spinnerVisible) {
        var spinner = $("div#spinner");
        spinner.stop();
        spinner.fadeOut("fast");
        spinnerVisible = false;
    }
};
//*********************End************************


function isNumberKey(evt, element) {
    var e = window.event || evt;
    //var charCode = (evt.which) ? evt.which : evt.keyCode
    var charCode = (window.Event) ? element.which : element.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode == 46 || charCode == 8) && charCode != undefined)
        return false;
    else {
        var len = $(element).val().length;
        var index = $(element).val().indexOf('.');
        if (index > 0 && charCode == 46) {
            return false;
        }
        if (index > 0) {
            var CharAfterdot = (len + 1) - index;
            if (CharAfterdot > 3) {
                return false;
            }
        }

    }
    return true;
}

function isNumberOnly(evt, element) {
    var e = window.event || evt;
    //var charCode = (evt.which) ? evt.which : evt.keyCode
    var charCode = (window.Event) ? element.which : element.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode == 8) && charCode != undefined)
        return false;
    else {
        var len = $(element).val().length;
        var index = $(element).val().indexOf('.');
        if (index > 0 && charCode == 46) {
            return false;
        }
        if (index > 0) {
            var CharAfterdot = (len + 1) - index;
            if (CharAfterdot > 3) {
                return false;
            }
        }

    }
    return true;
}
function onlyAlphabets(e, t) {

    var name = document.getElementById('txtName').value;
    try {

        if (window.event) {

            var charCode = window.event.keyCode;

        }

        else if (e) {

            var charCode = e.which;

        }

        else { return true; }

        if (charCode == 32 && name == "")

            return false;

        else

            return true;

    }

    catch (err) {

        alert(err.Description);

    }

}

function PhoneValidation(event) {
    var phone = document.getElementById('txtPhone').value;
    var k = event ? event.which : window.event.keyCode;
    if (k == 32 && phone == "") {
        return false;
    }
    else {
        return true;
    }
}
function EmailValidation(event) {
   
    var phone = document.getElementById('txtEmail').value;
    var k = event ? event.which : window.event.keyCode;
    if (k == 32 && phone == "") {
        return false;
    }
    else {
        return true;
    }
}
function DescriptionValidation(event) {
    var description = document.getElementById('txtComment').value;
    var k = event ? event.which : window.event.keyCode;
    if (k === 32 && description === "") {
        return false;
    }
    else {
        return true;
    }
}

function logError(str, actionName) {
    $.ajax({
        method:"POST",
        url: "/Admin/LogJqueryError",
        data: { err: str + actionName, actionName: ' Login Controller' }

    });
}