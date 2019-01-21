self.addEventListener('push', function (event) {
    console.log("Push triggered");
    console.log(event);

    event.waitUntil(
        self.registration.showNotification("My Notification", {
            body: "Notification"
        })
    )
});