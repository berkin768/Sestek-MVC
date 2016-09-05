if (!jQuery) { throw new Error("This page requires jQuery") }

var openContactWindow = function (id) {
    window.open('/Home/Contact/?id=' + id, "EditContactWindow", "width=350,height=600");
};

function delete_cookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

(function ($) {
    $(function () {
        var isInSestekDomain = $('#logoutButton').data('value');     
        if (isInSestekDomain === false) {        
            $("#logoutButton").css("display", "block");
        }
    });

    $(document).ready(function () {
        // fade in .navbar
        $(function () {
            $(window).scroll(function () {
                // set distance user needs to scroll before we start fadeIn
                if ($(this).scrollTop() > 40) {
                    $('.navbar').fadeOut();
                } else {
                    $('.navbar').fadeIn();
                }
            });
        });      
    });


}(jQuery));

(function ($) {
    var ajaxTable = $('#contactTableAjax')
        .DataTable({
            ajax: {
                type: 'POST',
                url: '/Home/GetAllContacts',
                data:
                    function (sendParam) {
                        sendParam.isAllContacts = !$('#phoneCheck').is(':checked');
                    },
                dataSrc: ''
            },

            "columns": [
                { "data": "displayName" },
                { "data": "telephoneNumber" },
                { "data": "title" },
                { "data": "physicalDeliveryOfficeName" },
                { "data": "mobile" },
                { "data": "mail" },
                { "data": "homePhone" },
                { "data": "isManual" }
            ],
            "oLanguage": {
                "sSearch": "Hızlı Ara",
                "sInfo": " _TOTAL_ kişiden  (_START_ - _END_) arası gösteriliyor",
                "sEmptyTable": "Tablo Boş",
                "sLoadingRecords": "Lütfen Bekleyin, Sayfa Yükleniyor...",
                "sZeroRecords": "Eşleşen Kayıt Bulunamadı",
                "sInfoFiltered": "[_MAX_ kayıt arasından filtrelendi]",
                "oPaginate": {
                    "sPrevious": "Önceki Sayfa",
                    "sNext": "Sonraki Sayfa"
                }
            },
            "columnDefs": [
                  { "className": "dt-left", "targets": [0, 2, 5] },
                {
                    "targets": 7,
                    "render": function (data, type, full) {
                        if (full && full.Id && full.isManual) {
                            return "<button onclick=openContactWindow(" + full.Id + ")>Düzenle</button>" + "<button id='delete'>Sil</button>";
                        }
                        return "LDAP";
                    }
                },
                {
                    "aTargets": [5],
                    "mData": "download_link",
                    "mRender": function (data, type, full) {
                        if (full.mail)
                            return "<a href=mailto:" + full.mail + ">" + full.mail + "</a>";
                        else
                            return "";
                    }
                }
            ],

            "pageLength": 25,
            dom: 'Bfrtip',
            buttons: [
                'excel', 'pdf', 'print',
                {
                    text: 'Yeni Kişi Ekle',
                    className: 'primary',
                    action: function () {
                        openContactWindow(0);
                    }
                }

            ]
        });


    $("#phoneCheck")
        .change(function () {
            ajaxTable.ajax.reload();
        });

    $('#displayName')
        .on('keyup',
            function () {
                ajaxTable.column(0).search(this.value).draw();
            });

    $('#title')
        .on('keyup',
            function () {
                ajaxTable.column(2).search(this.value).draw();
            });

    $('#office')
        .on('change',
            function () {
                ajaxTable.column(3).search(this.value).draw();
            });

    $('#telephone')
        .on('keyup',
            function () {
                ajaxTable.column(1).search(this.value).draw();
            });

    $('#email')
        .on('keyup',
            function () {
                ajaxTable.column(5).search(this.value).draw();
            });

    $('#logoutButton')
        .click(function () {  
            var flag = readCookie('rememberMe');  //Beni hatırla kapalı girildiyse
            debugger
            if (flag === "false") {                //Çıkış yapıldığında        
                delete_cookie('token1');    /* Çerezleri sil*/
                delete_cookie('token2');
                delete_cookie('rememberMe');
            }
            window.location.href = '/Login/Users';
        });

    $('#contactTableAjax')
        .on('click',
            '#delete',
           function () {
               if (confirm("Bu kaydı silmek istediğinize emin misiniz?")) {
                   var contact = ajaxTable.row($(this).parents('tr')).data();
                   $.ajax({
                       url: '/Home/DeleteContact',
                       type: 'POST',
                       data: { "Id": contact.Id }
                   });
                   ajaxTable.row($(this).parents('tr')).remove().draw();
                   ajaxTable.reload();
               }
           });

    $('#goToCreate')
        .click(function () {
            openContactWindow(0);
        });

})(jQuery);