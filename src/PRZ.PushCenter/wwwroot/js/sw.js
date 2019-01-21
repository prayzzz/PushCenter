self.addEventListener('push', event => {
    const pushMessage = event.data.json();

    event.waitUntil(
        self.registration.showNotification(pushMessage.Title, {
            body: pushMessage.Message,
            icon: pushMessage.IconUrl
        })
    )
});