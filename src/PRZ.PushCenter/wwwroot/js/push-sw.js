self.addEventListener('push', function (event) {
    var title = 'Yay a message.';
    var body = 'We have received a asd push message.';
    var icon = '/images/smiley.svg';
    var tag = 'simple-push-example-tag';
    event.waitUntil(
        self.registration.showNotification(title, {
            body: body,
            icon: icon,
            tag: tag
        })
    );
});