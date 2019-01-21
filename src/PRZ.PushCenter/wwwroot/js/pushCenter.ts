export class PushCenter {
    private readonly _isSupported: boolean;
    private pushServiceWorkerRegistration: Promise<ServiceWorkerRegistration>;

    constructor() {
        this._isSupported = "serviceWorker" in navigator && "PushManager" in window;

        if (this._isSupported) {
            this.registerServiceWorker();
        } else {
            console.debug("ServiceWorker not supported")
        }
    }

    get isSupported(): boolean {
        return this._isSupported;
    }

    private static urlB64ToUint8Array(base64String: String) {
        const padding = '='.repeat((4 - base64String.length % 4) % 4);
        const base64 = (base64String + padding)
            .replace(/\-/g, '+')
            .replace(/_/g, '/');

        const rawData = window.atob(base64);
        const outputArray = new Uint8Array(rawData.length);

        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    }

    public async subscribe(subscriptionType: number) {
        if (!this._isSupported) {
            return;
        }

        const publicKeyRequest = await fetch(`api/subscription/public-key`);
        if (!publicKeyRequest.ok) {
            console.error(`Couldn't retreive public-key`);
            return;
        }

        const publicKey = PushCenter.urlB64ToUint8Array(await publicKeyRequest.text());

        const options = {
            userVisibleOnly: true,
            applicationServerKey: publicKey
        };

        try {
            const pushSubscription = await this.pushServiceWorkerRegistration.then(r => r.pushManager.subscribe(options));
            console.debug("Subscription successful");

            const subscribeRequest = await fetch(`api/subscription?type=${subscriptionType}`, {
                method: 'post',
                headers: { "Content-type": "application/json" },
                body: JSON.stringify(pushSubscription)
            });

            if (subscribeRequest.ok) {
                console.debug("Subscription stored");
            } else {
                console.error(await subscribeRequest.text())
            }
        } catch (e) {
            console.error(e.message)
        }
    }

    public async unsubscribe(subscriptionType: number) {
        if (!this._isSupported) {
            return;
        }

        try {
            const pushSubscription = await this.pushServiceWorkerRegistration.then(r => r.pushManager.getSubscription());
            if (!pushSubscription) {
                return;
            }

            const req = await fetch(`api/subscription?type=${subscriptionType}`, {
                method: 'delete',
                headers: { "Content-type": "application/json" },
                body: JSON.stringify({ Endpoint: pushSubscription.endpoint })
            });

            if (req.ok) {
                console.debug("Subscription deleted");
            } else {
                console.error(await req.text())
            }
        } catch (e) {
            console.error(e.message)
        }
    }

    public async getSubscription(): Promise<PushSubscription | null> {
        return this.pushServiceWorkerRegistration.then(r => r.pushManager.getSubscription());
    }

    private registerServiceWorker() {
        if (!this._isSupported) {
            return;
        }

        this.pushServiceWorkerRegistration = navigator.serviceWorker.register('/js/sw.js', { scope: '/' });
    }
}
