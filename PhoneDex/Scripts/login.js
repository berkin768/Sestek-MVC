if (!jQuery) { throw new Error("This page requires jQuery") }

function delete_cookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

(function ($) {
    $('#rememberBox').prop("checked", true);
 

    var hasError = function () {
        if ($("#usernameBox").val() === '') {
            $("#usernameField").addClass('has-error');
            $('#usernameSpan').css("display", "block");
        } else {
            $("#usernameField").removeClass('has-error');
        }
        if ($("#passwordBox").val() === '') {
            $("#passwordField").addClass('has-error');
            $('#passwordSpan').css("display", "block");
        } else {
            $("#passwordField").removeClass('has-error');
        }
    };

    function loginOperations() {
        if (document.getElementById("rememberBox").checked)
            document.cookie = "rememberMe = true; path=/; expires=Thu, 18 Dec 2017 12:00:00 UTC";
        else
            document.cookie = "rememberMe = false; path=/; expires=Thu, 18 Dec 2017 12:00:00 UTC";

        if ($("#usernameBox").val() === '' || $("#passwordBox").val() === '') {
            hasError();
        } else
            $.post("/Login/Authentication",
                {
                    username: $("#usernameBox").val(),
                    password: $("#passwordBox").val()
                })
                .done(function (result) {
                    if (result.isBanned) {
                        var alertDialog = document.createElement('div');
                        alertDialog.id = 'alertDialog';
                        alertDialog.className = 'snackbar';
                        alertDialog.style.textAlign = "center";
                        alertDialog.innerHTML = "5 Defa Yanlış Girdiğiniz İçin 30 Dk Bekleyiniz, " +
                            "Son Deneme Tarihi : " +
                            result.availableTime;
                        parent = document.getElementById('content');
                        parent.appendChild(alertDialog);
                    } else if (result.success) {
                        window.location.href = result.location;
                    } else {
                        var snackbar = document.createElement('div');
                        snackbar.id = 'snackbar';
                        snackbar.className = 'snackbar';
                        snackbar.innerHTML = "Şifre ya da kullanıcı adı yanlış";
                        parent = document.getElementById('content');
                        parent.appendChild(snackbar);
                        $('.snackbar').fadeIn(400).delay(2000).fadeOut(400); //fade out after 3 seconds

                    }
                });
    }

    $(document)
        .ready(function() {
            $.post("/Login/RespondCookies",
            {})
                .done(function (data, status, headers, config) {                    
                    document.getElementById("usernameBox").value = data.username;                                                         
                    delete_cookie('token2');
                });
        });
 
    $("#LoginButton")
        .click(function () {
            loginOperations();
        });

    $(document).keypress(function (e) {
        if (e.which === 13) {
            loginOperations();
        }
    });

}(jQuery));
