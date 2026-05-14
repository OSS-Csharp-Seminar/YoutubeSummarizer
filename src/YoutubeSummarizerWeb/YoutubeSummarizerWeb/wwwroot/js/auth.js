window.authCookie = {
    set: function (value, days) {
        let expires = "";
        if (days) {
            const date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = "auth_session=" + encodeURIComponent(value) + expires + "; path=/; SameSite=Strict";
    },
    get: function () {
        const name = "auth_session=";
        const parts = document.cookie.split(';');
        for (let i = 0; i < parts.length; i++) {
            let c = parts[i].trim();
            if (c.indexOf(name) === 0) {
                return decodeURIComponent(c.substring(name.length));
            }
        }
        return null;
    },
    delete: function () {
        document.cookie = "auth_session=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Strict";
    }
};
