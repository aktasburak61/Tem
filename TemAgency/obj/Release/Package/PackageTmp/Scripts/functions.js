function baseUrl() {
    var href = window.location.href.split("/");
    var url = "";
    url = href[0] + "//" + href[2];
    return url;
}

$("#newLine").click(function () {
    var html = ""
    var j;
    if ($("#projectLine").find("tbody").find("tr").length == 0) {
        j = 0;
    }
    else {
        var j = $("#projectLine").find("tbody").find("tr").length;
    }
    html = html + '<tr id="rowNr_' + j + '">';
    html = html + '<td id="quantity_' + j + '"><input onchange="vatRateChanged(' + j + ')" type="text" class="form-control" style="font-size:12px!Important;text-align:right"/></td>';
    html = html + '<td id="unitPrice_' + j + '"><input onchange="vatRateChanged(' + j + ')" placeholder="0.0000 &#x20BA;" type="text" class="form-control" style="font-size:12px!Important;text-align:right"/></td>';
    html = html + '<td id="vatRate_' + j + '"><select onchange="vatRateChanged(' + j + ')" class="form-control  form-select" style="font-size:12px!Important;width:max-content;text-align:right"><option value="0">Seçiniz</option>';
    html = html + '<option value="1">0</option>';
    html = html + '<option value="2">1</option>';
    html = html + '<option value="3">8</option>';
    html = html + '<option value="4">18</option></select></td>';
    html = html + '<td id="total_' + j + '"><input type="text" class="form-control" disabled="disabled" style="font-size:12px!Important;text-align:right"/></td>';
    html = html + '<td id="vatTotal_' + j + '"><input type="text" class="form-control" disabled="disabled" style="font-size:12px!Important;text-align:right"/></td>';
    html = html + '<td id="netTotal_' + j + '"><input type="text" class="form-control" disabled="disabled" style="font-size:12px!Important;text-align:right"/></td>';
    html = html + '<td id="platform_' + j + '"></td>';
    html = html + '<td id="isLive_' + j + '"><select class="form-control  form-select" style="font-size:12px!Important;width:max-content"><option value="">Seçiniz</option>';
    html = html + '<option value="1">EVET</option>';
    html = html + '<option value="0">HAYIR</option></select></td>';
    html = html + '<td id="isAgency_' + j + '"><select class="form-control  form-select" onchange="isAgencyChanged(' + j + ')" style="font-size:12px!Important;width:max-content"><option value="2">Seçiniz</option>';
    html = html + '<option value="1">EVET</option >';
    html = html + '<option value="0">HAYIR</option></select></td>';
    html = html + '<td style="width:130px" id="agency_' + j + '"></td>';
    html = html + '<td id="influencer_' + j + '"></td>';
    html = html + '<td id="documentType_' + j + '"></td>';
    html = html + '<td id="buyPrice_' + j + '"><input onchange="buyPriceChanged(' + j + ')" placeholder="0.0000 &#x20BA;" type="text" class="form-control" style="font-size:12px!Important;text-align:right"/></td>';
    html = html + '<td style="min-width:80px;width:max-fit-content!important"><button class="btn btn-danger" onclick="deleteRow(' + j + ')"><i class="fa fa-remove"></i> Sil</button></td>';
    html = html + '</tr>';
    $("#projectLine").find("tbody").append(html);
    setComboBox(j);
});

function setComboBox(j) {
    $.getJSON(baseUrl() + '/Projects/GetPlatform', function (result) {
        $('#platform_' + j).html("");
        var data = result.data;
        $('#platform_' + j).append("<select class='form-control form-select' style='font-size:12px!Important;width:max-content'></select>");
        for (var i = 0; i < data.length; i++) {
            $('#platform_' + j).find("select").append("<option value='" + data[i].LOGICALREF + "'>" + data[i].DESCRIPTION + "</option>");
        }
    })

    $.getJSON(baseUrl() + '/Projects/GetInfluencer', function (result) {
        $("#influencer_" + j).html("");
        var data = result.data;
        $("#influencer_" + j).append("<select class='form-control form-select' onchange='influencerChanged(" + j + ")' style='font-size:12px!Important;width:max-content'></select>");
        for (var i = 0; i < data.length; i++) {
            $("#influencer_" + j).find("select").append("<option value='" + data[i].LOGICALREF + "'>" + data[i].DESCRIPTION + "</option>");
        }
    })

    $.getJSON(baseUrl() + '/Projects/GetAgency', function (result) {
        $("#agency_" + j).html("");
        var data = result.data;
        $("#agency_" + j).append("<select class='form-control form-select' disabled='disabled' style='font-size:12px!Important;width:130px'></select>");
        for (var i = 0; i < data.length; i++) {
            $("#agency_" + j).find("select").append("<option value='" + data[i].LOGICALREF + "'>" + data[i].DESCRIPTION + "</option>");
        }
    })

    $.getJSON(baseUrl() + '/Projects/GetDocumentType', function (result) {
        $("#documentType_" + j).html("");
        var data = result.data;
        $("#documentType_" + j).append("<select class='form-control form-select' style='font-size:12px!Important;width:max-content'></select>");
        for (var i = 0; i < data.length; i++) {
            $("#documentType_" + j).find("select").append("<option value='" + data[i].LOGICALREF + "'>" + data[i].DESCRIPTION + "</option>");
        }
    })
}

function deleteRow(j) {
    $("#rowNr_" + j).remove();
}

$("#sectorList").on("change", function () {
    showValue($(this).val());
})

function showValue(val) {
    if (val === "") {
        $("#subSectorList").attr('disabled', 'disabled');
    }
    else {
        $("#subSectorList").removeAttr('disabled');
    }
    $.getJSON(baseUrl() + '/Projects/GetSubSector' + "?number=" + val, function (result) {
        $("#subSectorList").html("");
        var data = result.data;
        for (var i = 0; i < data.length; i++) {
            $("#subSectorList").append("<option value='" + data[i].Id + "'>" + data[i].Description + "</option>");
        }
    })
}

function isAgencyChanged(j) {
    var agency = parseInt($("#isAgency_" + j).find("option:selected").val());
    if (agency == 1) {
        $("#agency_" + j).find("select").removeAttr('disabled');
    }
    else {
        $("#agency_" + j).find("select").attr('disabled', 'disabled');
    }
}

function influencerChanged(j) {
    var influencer = parseInt($("#influencer_" + j).find("option:selected").val());
    if (influencer != 0) {
        $.getJSON(baseUrl() + '/Projects/GetDocIdByInfluencerId' + "?number=" + influencer, function (result) {
            var data = result.data;
            $("#documentType_" + j).find("select").val(data).change();
        })
    }
    else {
        $("#documentType_" + j).find("select").val("0").change();
    }
}

function vatRateChanged(j) {
    $("#unitPrice_" + j).find("input").val($("#unitPrice_" + j).find("input").val().replace(",", "."));
    var unitPrice = parseFloat($("#unitPrice_" + j).find("input").val());
    var unitPriceStr = unitPrice.toFixed(4);
    var quantity = parseInt($("#quantity_" + j).find("input").val());
    var vat = parseInt($("#vatRate_" + j).find("option:selected").val());
    var vatRate = parseInt($("#vatRate_" + j).find("option:selected").text());
    var total = (unitPrice * quantity);
    var vatTotal = (((unitPrice * quantity) / 100) * vatRate);
    var netTotal = total + vatTotal;
    if (unitPriceStr > 0) {
        $("#unitPrice_" + j).find("input").val(unitPriceStr);
        if (quantity > 0) {
            if (vat > 0) {
                $("#total_" + j).find("input").val(total.toFixed(4));
                $("#vatTotal_" + j).find("input").val(vatTotal.toFixed(4));
                $("#netTotal_" + j).find("input").val(netTotal.toFixed(4));
            }
            else if (vat == 0) {
                $("#total_" + j).find("input").val(vat);
                $("#vatTotal_" + j).find("input").val(vat);
                $("#netTotal_" + j).find("input").val(vat);
            }
        }
    }
}

function buyPriceChanged(j) {
    $("#buyPrice_" + j).find("input").val($("#buyPrice_" + j).find("input").val().replace(",", "."));
    var buyPrice = parseFloat($("#buyPrice_" + j).find("input").val());
    var buyPriceStr = buyPrice.toFixed(4);
    $("#buyPrice_" + j).find("input").val(buyPriceStr);
}

function agencyChanged() {
    var agency = $("#project > tbody").children("tr").eq(4).children("td").eq(0).find("option:selected").text();
    if (agency == "Ajans") {
        $.getJSON(baseUrl() + '/Projects/GetAgency', function (result) {
            var data = result.data;
            $("#project > tbody").append('<tr><th>Ajans</th><td></td></tr>')
            $("#project > tbody").children("tr").eq(6).children("td").eq(0).append("<select class='form-control form-select' style='font-size:12px!Important' id='AgencyId'></select>");
            for (var i = 0; i < data.length; i++) {
                $("#project > tbody").children("tr").eq(6).children("td").eq(0).find("select").append("<option value='" + data[i].LOGICALREF + "'>" + data[i].DESCRIPTION + "</option>");
            }
        })
    }
    else {
        $("#project > tbody").children("tr").eq(6).remove();
        $("#project > tbody").children("tr").eq(6).remove();
    }
}

function saveProject() {
    var projectLine = new Array();
    var projectModel = {
        Id: "", ProjectCode: "", ProjectDate: "", Origin: "", BrandId: 0, Brand: "", SectorId: 0, Sector: "", UserId: 0, SubSectorId: 0, SubSector: "", Agency: "",
        CustomerNameId: 0, CustomerName: "", AgencyId: 0, AgencyName: "", ProjectName: "", CompanyId: 0, Company: "", ProjectLines: []
    };
    $("#project > tbody").each(function (i, j) {
        var id = 0;
        var projectCode = $(j).children("tr").eq(0).children("td").eq(0).find("input").val();
        var projectName = $(j).children("tr").eq(0).children("td").eq(2).find("input").val();
        var companyId = $(j).children("tr").eq(0).children("td").eq(4).find("option:selected").val();
        var company = $(j).children("tr").eq(0).children("td").eq(4).find("option:selected").text();
        var projectDate = $(j).children("tr").eq(1).children("td").eq(0).find("input").val();
        var origin = $(j).children("tr").eq(1).children("td").eq(2).find("option:selected").text();
        var brandId = $(j).children("tr").eq(1).children("td").eq(4).find("option:selected").val();
        var brand = $(j).children("tr").eq(1).children("td").eq(4).find("option:selected").text();
        var sectorId = $(j).children("tr").eq(2).children("td").eq(0).find("option:selected").val();
        var sector = $(j).children("tr").eq(2).children("td").eq(0).find("option:selected").text();
        var user = $(j).children("tr").eq(2).children("td").eq(2).children("input").eq(1).val();
        var subSectorId = $(j).children("tr").eq(3).children("td").eq(0).find("option:selected").val();
        var subSector = $(j).children("tr").eq(3).children("td").eq(0).find("option:selected").text();
        var agencyProject = $(j).children("tr").eq(4).children("td").eq(0).find("option:selected").text();
        var customerNameId = $(j).children("tr").eq(5).children("td").eq(0).find("option:selected").val();
        var customerName = $(j).children("tr").eq(5).children("td").eq(0).find("option:selected").text();
        var agencyId = $(j).children("tr").eq(6).children("td").eq(0).find("option:selected").val();
        var agencyName = $(j).children("tr").eq(6).children("td").eq(0).find("option:selected").text();
        projectModel.Id = id;
        projectModel.ProjectCode = projectCode;
        projectModel.ProjectName = projectName;
        projectModel.CompanyId = companyId;
        projectModel.Company = company;
        projectModel.ProjectDate = projectDate;
        projectModel.Origin = origin;
        projectModel.BrandId = brandId;
        projectModel.Brand = brand;
        projectModel.SectorId = sectorId;
        projectModel.Sector = sector;
        projectModel.UserId = user;
        projectModel.SubSectorId = subSectorId;
        projectModel.SubSector = subSector;
        projectModel.Agency = agencyProject;
        projectModel.CustomerNameId = customerNameId;
        projectModel.CustomerName = customerName;
        projectModel.AgencyId = agencyId;
        projectModel.AgencyName = agencyName;
    })

    $("#projectLine").find("tbody").find("tr").each(function (i, j) {
        var quantity = parseInt($(j).children("td").eq(0).find("input").val());
        var unitPrice = parseFloat($(j).children("td").eq(1).find("input").val());
        var vatRate = $(j).children("td").eq(2).find("option:selected").text();
        var total = parseFloat($(j).children("td").eq(3).find("input").val());
        var vatTotal = parseFloat($(j).children("td").eq(4).find("input").val());
        var netTotal = parseFloat($(j).children("td").eq(5).find("input").val());
        var platformId = parseInt($(j).children("td").eq(6).find("option:selected").val());
        var platform = $(j).children("td").eq(6).find("option:selected").text();
        var live = parseInt($(j).children("td").eq(7).find("option:selected").val());
        var isAgency = parseInt($(j).children("td").eq(8).find("option:selected").val());
        var agencyId = parseInt($(j).children("td").eq(9).find("option:selected").val());
        var agency = $(j).children("td").eq(9).find("option:selected").text();
        var influencerId = parseInt($(j).children("td").eq(10).find("option:selected").val());
        var influencer = $(j).children("td").eq(10).find("option:selected").text();
        var documentTypeId = parseInt($(j).children("td").eq(11).find("option:selected").val());
        var documentType = $(j).children("td").eq(11).find("option:selected").text();
        var buyPrice = parseFloat($(j).children("td").eq(12).find("input").val());
        var projectLineModel = {
            Quantity: quantity, UnitPrice: unitPrice, VatRate: vatRate, Total: total, VatTotal: vatTotal, NetTotal: netTotal,
            PlatformId: platformId, Platform: platform, Live: live, IsAgency: isAgency, AgencyId: agencyId, Agency: agency, InfluencerId: influencerId, Influencer: influencer,
            DocumentTypeId: documentTypeId, DocumentType: documentType, BuyPrice: buyPrice
        };
        projectLine.push(projectLineModel);
    });
    projectModel.ProjectLines = projectLine;
    var errorCount = 0;
    var errorLineCount = 0;
    function checkProjectValidation() {
        if (projectModel.Origin === "Menşei Seçiniz") {
            $("#Origin").next("span").remove();
            $("#Origin").after("<span style='color:red;font-size:10px'>Lütfen Menşei Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#Origin").next("span").remove();
        }
        if (projectModel.Brand === "Marka Seçiniz") {
            $("#Brand").next("span").remove();
            $("#Brand").after("<span style='color:red;font-size:10px'>Lütfen Marka Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#Brand").next("span").remove();
        }
        if (projectModel.Sector === "Sektör Seçiniz") {
            $("#sectorList").next("span").remove();
            $("#sectorList").after("<span style='color:red;font-size:10px'>Lütfen Sektör Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#sectorList").next("span").remove();
        }
        if (projectModel.SubSector === "Alt Sektör Seçiniz") {
            $("#subSectorList").next("span").remove();
            $("#subSectorList").after("<span style='color:red;font-size:10px'>Lütfen Alt Sektör Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#subSectorList").next("span").remove();
        }
        if (projectModel.Agency === "Seçiniz") {
            $("#Agency").next("span").remove();
            $("#Agency").after("<span style='color:red;font-size:10px'>Lütfen Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#Agency").next("span").remove();
        }
        if (projectModel.CustomerName === "Müşteri Seçiniz") {
            $("#CustomerNameId").next("span").remove();
            $("#CustomerNameId").after("<span style='color:red;font-size:10px'>Lütfen Müşteri Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#CustomerNameId").next("span").remove();
        }
        if (projectModel.Agency === "Ajans" && projectModel.AgencyId === "Ajans Seçiniz") {
            $("#AgencyId").next("span").remove();
            $("#AgencyId").after("<span style='color:red;font-size:10px'>Lütfen Ajans Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#AgencyId").next("span").remove();
        }
        if (projectModel.ProjectName === "") {
            $("#ProjectName").next("span").remove();
            $("#ProjectName").after("<span style='color:red;font-size:10px'>Lütfen Proje Adı Giriniz.</span>");
            errorCount += 1;
        }
        else {
            $("#ProjectName").next("span").remove();
        }
        if (projectModel.Company === "Firma Seçiniz") {
            $("#Company").next("span").remove();
            $("#Company").after("<span style='color:red;font-size:10px'>Lütfen Firma Seçiniz.</span>");
            errorCount += 1;
        }
        else {
            $("#Company").next("span").remove();
        }
    }
    function checkProjectLineValidation() {
        projectLine.forEach(function (i, j) {
            if (isNaN(projectLine[j].Quantity)) {
                $("#quantity_" + j).find("input").next("span").remove();
                $("#quantity_" + j).find("input").after("<span style='color:red;font-size:10px'>Lütfen Miktar Giriniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#quantity_" + j).find("input").next("span").remove();
            }
            if (isNaN(projectLine[j].UnitPrice)) {
                $("#unitPrice_" + j).find("input").next("span").remove();
                $("#unitPrice_" + j).find("input").after("<span style='color:red;font-size:10px'>Lütfen Birim Fiyat Giriniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#unitPrice_" + j).find("input").next("span").remove();
            }
            if (projectLine[j].VatRate == "Seçiniz") {
                $("#vatRate_" + j).find("select").next("span").remove();
                $("#vatRate_" + j).find("select").after("<span style='color:red;font-size:10px'>Lütfen KDV Oranı Seçiniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#vatRate_" + j).find("select").next("span").remove();
            }
            if (projectLine[j].Platform == "Platform Seçiniz") {
                $("#platform_" + j).find("select").next("span").remove();
                $("#platform_" + j).find("select").after("<span style='color:red;font-size:10px'>Lütfen Platform Seçiniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#platform_" + j).find("select").next("span").remove();
            }
            if (projectLine[j].Live != 1 && projectLine[j].Live != 0) {
                $("#isLive_" + j).find("select").next("span").remove();
                $("#isLive_" + j).find("select").after("<span style='color:red;font-size:10px'>Lütfen Seçiniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#isLive_" + j).find("select").next("span").remove();
            }
            if (projectLine[j].IsAgency != 1 && projectLine[j].IsAgency != 0) {
                $("#isAgency_" + j).find("select").next("span").remove();
                $("#isAgency_" + j).find("select").after("<span style='color:red;font-size:10px'>Lütfen Seçiniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#isAgency_" + j).find("select").next("span").remove();
            }
            if (projectLine[j].IsAgency == 1 && projectLine[j].Agency == "Ajans Seçiniz") {
                $("#agency_" + j).find("select").next("span").remove();
                $("#agency_" + j).find("select").after("<span style='color:red;font-size:10px'>Lütfen Ajans Seçiniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#agency_" + j).find("select").next("span").remove();
            }
            if (projectLine[j].Influencer == "Influencer Seçiniz") {
                $("#influencer_" + j).find("select").next("span").remove();
                $("#influencer_" + j).find("select").after("<span style='color:red;font-size:10px'>Lütfen Influencer Seçiniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#influencer_" + j).find("select").next("span").remove();
            }
            if (projectLine[j].DocumentType == "Seçiniz") {
                $("#documentType_" + j).find("select").next("span").remove();
                $("#documentType_" + j).find("select").after("<span style='color:red;font-size:10px'>Lütfen Belge Türü Seçiniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#documentType_" + j).find("select").next("span").remove();
            }
            if (isNaN(projectLine[j].BuyPrice)) {
                $("#buyPrice_" + j).find("input").next("span").remove();
                $("#buyPrice_" + j).find("input").after("<span style='color:red;font-size:10px'>Lütfen Alış Tutarı Giriniz.</span>");
                errorLineCount += 1;
            }
            else {
                $("#buyPrice_" + j).find("input").next("span").remove();
            }
        })
    }
    checkProjectLineValidation();
    checkProjectValidation();
    if (errorCount == 0 && errorLineCount == 0) {
        $.ajax({
            type: "POST",
            url: baseUrl() + "/Projects/SaveProject",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(projectModel),
            traditional: true,
            success: function (data) {
                if (data) {
                    window.location.href = baseUrl() + "/Projects/AllProjects"
                }
                else {
                    alert("Kayıt Yapılamadı.")
                }
            }
        });
    }
}

function deleteSector(sectorLogRef, sectorCode, sectorName) {
    $("#sectorCode").html(sectorCode);
    $("#sectorName").html(sectorName);
    var a = $("#sectorDeleteForm").attr("action");
    $("#sectorDeleteForm").attr("action", a + "/?number=" + sectorLogRef);
}

function deleteBrand(brandLogRef, brandName) {
    $("#brandName").html(brandName);
    var a = $("#brandDeleteForm").attr("action");
    $("#brandDeleteForm").attr("action", a + "/?number=" + brandLogRef);
}

function deleteSubSector(subSectorLogRef, subSectorCode, subSectorName) {
    $("#subSectorCode").html(subSectorCode);
    $("#subSectorName").html(subSectorName);
    var a = $("#subSectorDeleteForm").attr("action");
    $("#subSectorDeleteForm").attr("action", a + "/?number=" + subSectorLogRef);
}

function sendToLogo(ProjectId) {
    $.ajax({
        type: 'POST',
        contentType: "application/json;",
        dataType: "json",
        url: baseUrl() + "/Projects/SendToLogo",
        data: '{ "ProjectId": ' + ProjectId + '}',
        success: function (data) {
            window.location.href = baseUrl() + "/Projects/AllProjects"
        },
        error: function (data) {
        }
    });
}

function deleteDocument(number) {
    $.ajax({
        type: "POST",
        url: baseUrl() + "/Projects/DeleteDocument",
        contentType: 'application/json; charset=utf-8',
        data: '{ "documentId": ' + number + '  }',
        dataType: 'json',
        success: function (data) {
            if (data) {
                location.reload();
            } else {
                alert("Dosya Silinemedi ...")
            }
        },
        error: function (req, status, err) {
            alert('err=' + err + ' status=' + status);
        }
    });
}

function deleteDocumentProjectsAgreement(number) {
    $.ajax({
        type: "POST",
        url: baseUrl() + "/ProjectsAgreement/DeleteDocumentProjectsAgreement",
        contentType: 'application/json; charset=utf-8',
        data: '{ "documentId": ' + number + '  }',
        dataType: 'json',
        success: function (data) {
            if (data) {
                window.location.href = window.location.href;
            } else {
                alert("Dosya Silinemedi ...")
            }
        },
        error: function (req, status, err) {
            alert('err=' + err + ' status=' + status);
        }
    });
}

function SelectStep(rowNr, lineId) {
    var color = $('#btnSelectStep_' + rowNr + lineId).css("background-color").toString();
    var projectId = $("#step_Id").val();
    var isSelect = false;
    if (color == 'rgb(30, 144, 255)') {
        $('#btnSelectStep_' + rowNr + "" + lineId).css("background-color", "white");
        isSelect = false;
    }
    else {
        $('#btnSelectStep_' + rowNr + "" + lineId).css("background-color", "dodgerblue");
        isSelect = true;
    }
    //$.ajax({
    //    type: 'Post',
    //    contentType: "application/json;",
    //    dataType: "json",
    //    url: baseUrl() + "/Projects/SelectSupplier",
    //    data: '{ "supplierRef": ' + supplierRef + ',"isSelect": ' + isSelect + ',"ProjectId": ' + projectId + '}',
    //    success: function (data) {
    //    },
    //    error: function (data) {
    //    }
    //});
}

function SaveOperation() {
    var projectLineSteps = new Array();
    $("#lineOperation").find("table").each(function (i, j) {
        $(j).find("tbody").each(function (m, n) {
            $(n).find("tr").each(function (k, l) {
                var lineSelected = $(l).children("td").eq(0).find("button").css("background-color");
                var status = 0;
                if (lineSelected == 'rgb(255, 255, 255)') {
                }
                else if (lineSelected == 'rgb(30, 144, 255)') {
                    var id = parseInt($(l).find("#step_Id").val());
                    var projectLineId = parseInt($(l).find("#step_ProjectLineId").val());
                    var stepDate = $(l).children("td").eq(3).find("input").val();
                    var explanation = $(l).children("td").eq(4).find("input").val();
                    var link = $(l).children("td").eq(5).find("input").val();
                    var priceStr = $(l).children("td").eq(6).find("input").val().replace(",", ".");
                    var price = parseFloat(priceStr);
                    var stepStatus = $(l).children("td").eq(2).find("option:selected").text();
                    if (stepStatus == "Durum Seçiniz") {
                        status = 0;
                    }
                    else if (stepStatus == "Bekliyor") {
                        status = 1;
                    }
                    else if (stepStatus == "Tamamlandı") {
                        status = 2;
                    }
                    else if (stepStatus == "Logoya Aktarıldı") {
                        status = 3;
                    }
                    var projectLineStep = {
                        Id: id, ProjectLineId: projectLineId, Status: status, StepDate: stepDate, Explanation: explanation, Link: link, Price: price
                    };
                    projectLineSteps.push(projectLineStep);
                }
            });
        });
    });
    $.ajax({
        type: "POST",
        url: baseUrl() + "/Projects/SaveOperation",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(projectLineSteps),
        traditional: true,
        success: function (data) {
            if (data) {
                window.location.href = window.location.href;
            }
            else {
                alert("Kayıt Yapılamadı.")
            }
        }
    });
}

function CancelProject(number) {
    var message = $("#lExp").val();
    $.ajax({
        type: "POST",
        url: baseUrl() + "/ProjectsAgreement/CancelProject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ number: number, lExp: message }),
        traditional: true,
        success: function (data) {
            if (data) {
                window.location.href = data;
            }
            else {
                alert("Kayıt Yapılamadı.")
            }
        }
    });
}

function EditProject(number) {
    var message = $("#lExp").val();
    $.ajax({
        type: "POST",
        url: baseUrl() + "/ProjectsAgreement/EditProject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ number: number, lExp: message }),
        traditional: true,
        success: function (data) {
            if (data) {
                window.location.href = data;
            }
            else {
                alert("Kayıt Yapılamadı.")
            }
        }
    });
}

function SendEditProject(number) {
    var message = $("#Explanation").val();
    $.ajax({
        type: "POST",
        url: baseUrl() + "/ProjectsAgreement/SendEditProject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ number: number, Explanation: message }),
        traditional: true,
        success: function (data) {
            if (data) {
                window.location.href = data;
            }
            else {
                alert("Kayıt Yapılamadı.")
            }
        }
    });
}

function AcceptProject(number) {
    var message = $("#lExp").val();
    $.ajax({
        type: "POST",
        url: baseUrl() + "/ProjectsAgreement/AcceptProject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ number: number, lExp: message }),
        traditional: true,
        success: function (data) {
            if (data) {
                window.location.href = data;
            }
            else {
                alert("Kayıt Yapılamadı.")
            }
        }
    });
}

function FinanceAcceptProject(number) {
    var message = $("#FExplanation").val();
    var tax = $("#StampTax").find("option:selected").val();
    if ($("#StampTax").find("option:selected").text() === "Seçiniz") {
        $("#StampTax").after("<br/><span style='color:red;font-size:10px'>Lütfen Damga Vergisi Durumu Seçiniz.</span>");
    }
    else {
        $.ajax({
            type: "POST",
            url: baseUrl() + "/ProjectsAgreement/FinanceAcceptProject",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ number: number, fExplanation: message, stampTax: tax }),
            traditional: true,
            success: function (data) {
                if (data) {
                    window.location.href = data;
                }
                else {
                    alert("Kayıt Yapılamadı.")
                }
            }
        });
    }
}

function FinanceCancelProject(number) {
    var message = $("#FExplanation").val();
    var tax = $("#StampTax").find("option:selected").val();
    if ($("#StampTax").find("option:selected").text() === "Seçiniz") {
        $("#StampTax").after("<br/><span style='color:red;font-size:10px'>Lütfen Damga Vergisi Durumu Seçiniz.</span>");
    }
    else {
        $.ajax({
            type: "POST",
            url: baseUrl() + "/ProjectsAgreement/FinanceCancelProject",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ number: number, fExplanation: message, stampTax: tax }),
            traditional: true,
            success: function (data) {
                if (data) {
                    window.location.href = data;
                }
                else {
                    alert("Kayıt Yapılamadı.")
                }
            }
        });
    }
}

function agreementStatusChange(number) {
    var value = document.getElementById("agreementStatus").value;
    $.ajax({
        type: "POST",
        url: baseUrl() + "/ProjectsAgreement/ChangeAgreementStatus",
        contentType: "application/json",
        dataType: "json",
        data: '{ "number": ' + number + ', "value": ' + value + '  }',
        success: function (data) {
            if (data) {
                window.location.href = window.location.href;
            }
            else {
                alert("Kayıt Yapılamadı.")
            }
        }
    });
}

function projectStatusChange(number) {
    var value = document.getElementById("projectStatus").value;
    $.ajax({
        type: "POST",
        url: baseUrl() + "/Projects/ChangeProjectStatus",
        contentType: "application/json",
        dataType: "json",
        data: '{ "number": ' + number + ', "value": ' + value + '  }',
        success: function (data) {
            if (data) {
                window.location.href = window.location.href;
            }
            else {
                alert("Kayıt Yapılamadı.")
            }
        }
    });
}

function sendConfirmEmail(number) {
    $.ajax({
        type: "POST",
        url: baseUrl() + "/User/SendConfirmEmail?userId="+number,
        contentType: "application/text",
        dataType: "text",
        data:  number,
        success: function (data) {
            if (data) {
                alert("Onay maili gönderildi.")
            }
            else {
                alert("Mail gönderilemedi.")
            }
        }
    });
}