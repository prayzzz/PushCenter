"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.PushCenter = void 0;
class PushCenter {
    constructor() {
        this.pushServiceWorkerRegistration = null;
        this._isSupported = "serviceWorker" in navigator && "PushManager" in window;
        if (this._isSupported) {
            this.registerServiceWorker();
        }
        else {
            console.debug("ServiceWorker not supported");
        }
    }
    get isSupported() {
        return this._isSupported;
    }
    static urlB64ToUint8Array(base64String) {
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
    subscribe(subscriptionType) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!this._isSupported) {
                return;
            }
            const publicKeyRequest = yield fetch(`api/subscription/public-key`);
            if (!publicKeyRequest.ok) {
                console.error(`Couldn't retreive public-key`);
                return;
            }
            const publicKey = PushCenter.urlB64ToUint8Array(yield publicKeyRequest.text());
            const options = {
                userVisibleOnly: true,
                applicationServerKey: publicKey
            };
            try {
                const pushSubscription = yield this.pushServiceWorkerRegistration.then(r => r.pushManager.subscribe(options));
                console.debug("Subscription successful");
                const subscribeRequest = yield fetch(`api/subscription?type=${subscriptionType}`, {
                    method: 'post',
                    headers: { "Content-type": "application/json" },
                    body: JSON.stringify(pushSubscription)
                });
                if (subscribeRequest.ok) {
                    console.debug("Subscription stored");
                }
                else {
                    console.error(yield subscribeRequest.text());
                }
            }
            catch (e) {
                console.error(e.message);
            }
        });
    }
    unsubscribe(subscriptionType) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!this._isSupported) {
                return;
            }
            try {
                const pushSubscription = yield this.pushServiceWorkerRegistration.then(r => r.pushManager.getSubscription());
                if (!pushSubscription) {
                    return;
                }
                const req = yield fetch(`api/subscription?type=${subscriptionType}`, {
                    method: 'delete',
                    headers: { "Content-type": "application/json" },
                    body: JSON.stringify({ Endpoint: pushSubscription.endpoint })
                });
                if (req.ok) {
                    console.debug("Subscription deleted");
                }
                else {
                    console.error(yield req.text());
                }
            }
            catch (e) {
                console.error(e.message);
            }
        });
    }
    getSubscription() {
        return __awaiter(this, void 0, void 0, function* () {
            return this.pushServiceWorkerRegistration.then(r => r.pushManager.getSubscription());
        });
    }
    registerServiceWorker() {
        if (!this._isSupported) {
            return;
        }
        this.pushServiceWorkerRegistration = navigator.serviceWorker.register('/dist/sw.js', { scope: '/' });
    }
}
exports.PushCenter = PushCenter;
//# sourceMappingURL=pushCenter.js.map