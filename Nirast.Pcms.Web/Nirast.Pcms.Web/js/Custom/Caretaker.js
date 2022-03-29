var certificateFiles = [];
var SinFiles = [];
var OtherFiles = [];


function removeCertificateFile(id) {
    var index = certificateFiles.indexOf(certificateFiles.filter(x => x.id == id)[0]);
    if (index > -1) {
        certificateFiles.splice(index, 1);
    }
    addCertificateHtml();
    showProgress();
    $.ajax({
        type: "POST",
        url: '/Caretaker/RemoveCertificateFiles/' + id,
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result);
            spinnerVisible = true;
            hideProgress();
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
            spinnerVisible = true;
            hideProgress();
            logError(status + ' - Error occurred while removing the Caregiver Certificate. Function: removeCertificateFile(id). Page: Caretaker.js');
        }
    });
}

function addCertificateHtml() {
    $("#selectedCertificateFiles").html("");
    for (var i = 0; i < certificateFiles.length; ++i) {
        fileNames = "<div class='un-group-selected'><span>" + certificateFiles[i].name + "</span><a onclick='removeCertificateFile(" + certificateFiles[i].id + ")'>x</a></div>";
        $('#selectedCertificateFiles').append(fileNames);
    }
}

function removeSinFile(id) {
    var index = SinFiles.indexOf(SinFiles.filter(x => x.id == id)[0]);
    if (index > -1) {
        SinFiles.splice(index, 1);
    }
    addSinHtml();
    showProgress();
    $.ajax({
        type: "POST",
        url: '/Caretaker/RemoveSinFiles/' + id,
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result);
            spinnerVisible = true;
            hideProgress();
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
            spinnerVisible = true;
            hideProgress();
            logError(status + ' - Error occurred while removing the Caregiver SIN Files. Function: removeSinFile(id). Page: Caretaker.js');
        }
    });
}

function addSinHtml() {
    $("#selectedSinFiles").html("");
    for (var i = 0; i < SinFiles.length; ++i) {
        fileNames = "<div class='un-group-selected'><span>" + SinFiles[i].name + "</span><a onclick='removeSinFile(" + SinFiles[i].id + ")'>x</a></div>";
        $('#selectedSinFiles').append(fileNames);
    }
}

function removeOtherFile(id) {
    var index = OtherFiles.indexOf(OtherFiles.filter(x => x.id == id)[0]);
    if (index > -1) {
        OtherFiles.splice(index, 1);
    }
    addOtherHtml();
    showProgress();
    $.ajax({
        type: "POST",
        url: '/Caretaker/RemoveOtherFiles/' + id,
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result);
            spinnerVisible = true;
            hideProgress();
        },
        error: function (xhr, status, p3, p4) {
            spinnerVisible = true;
            hideProgress();
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
            logError(status + ' - Error occurred while removing the Caregiver Other Files. Function: removeOtherFile(id). Page: Caretaker.js');
        }
    });
}

function addOtherHtml() {
    $("#selectedOtherFiles").html("");
    for (var i = 0; i < OtherFiles.length; ++i) {
        fileNames = "<div class='un-group-selected'><span>" + OtherFiles[i].name + "</span><a onclick='removeOtherFile(" + OtherFiles[i].id + ")'>x</a></div>";
        $('#selectedOtherFiles').append(fileNames);
    }
}


$(document).ready(function () {
    $('#CertificateFiles').on('change', function (e) {
        var files = e.target.files;
        //var myID = 3; //uncomment this to make sure the ajax URL works
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    var id = certificateFiles.length + 1;
                    certificateFiles.push({ id: id, file: e.target.files[x], name: e.target.files[x].name });
                    data.append("file_" + id, files[x]);
                }
                addCertificateHtml();
                showProgress();
                $.ajax({
                    type: "POST",
                    url: '/Caretaker/UploadCertificateFiles',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        $('#lblCertificateFiles').removeClass('required-border');
                        console.log(result);
                        spinnerVisible = true;
                        hideProgress();
                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                        spinnerVisible = true;
                        hideProgress();
                        logError(status + ' - Error occurred while Uploading the Caregiver Certificates. Function: $("#CertificateFiles").on("change", function (e). Page: Caretaker.js');
                    }
                });
            }
            else {
                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });

    $('#SinFiles').on('change', function (e) {
        var files = e.target.files;
        //var myID = 3; //uncomment this to make sure the ajax URL works
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    var id = SinFiles.length + 1;
                    SinFiles.push({ id: id, file: e.target.files[x], name: e.target.files[x].name });
                    data.append("file_" + id, files[x]);
                }
                addSinHtml();
                showProgress();
                $.ajax({
                    type: "POST",
                    url: '/Caretaker/UploadSinFiles',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        $('#lblSINFile').removeClass('required-border');
                        console.log(result);
                        spinnerVisible = true;
                        hideProgress();
                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                        spinnerVisible = true;
                        hideProgress();
                        logError(status + ' - Error occurred while Uploading the Caregiver SIN Files. Function: $("#SinFiles").on("change", function (e). Page: Caretaker.js');
                    }
                });
            }
            else {
                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });

    $('#OtherFiles').on('change', function (e) {
        var files = e.target.files;
        //var myID = 3; //uncomment this to make sure the ajax URL works
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    var id = OtherFiles.length + 1;
                    OtherFiles.push({ id: id, file: e.target.files[x], name: e.target.files[x].name });
                    data.append("file_" + id, files[x]);
                }
                addOtherHtml();
                showProgress();
                $.ajax({
                    type: "POST",
                    url: '/Caretaker/UploadOtherFiles',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        $('#lblOtherDocuments').removeClass('required-border');
                        console.log(result);
                        spinnerVisible = true;
                        hideProgress();
                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                        spinnerVisible = true;
                        hideProgress();
                        logError(status + ' - Error occurred while Uploading the Caregiver Other Files. Function: $("#OtherFiles").on("change", function (e). Page: Caretaker.js');
                    }
                });
            }
            else {
                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });
});