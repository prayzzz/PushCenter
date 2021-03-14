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
const pushCenter_1 = require("pushCenter");
const subscriptionService_1 = require("subscriptionService");
const pushCenter = new pushCenter_1.PushCenter();
const subscriptionService = new subscriptionService_1.SubscriptionService();
window.PushCenter = pushCenter;
if (pushCenter.isSupported) {
    renderSubs();
}
else {
    renderNotSupported();
}
function renderNotSupported() {
    const loading = document.querySelector("#loading");
    loading.style.display = "none";
    const subscriptions = document.querySelector("#subscriptions");
    subscriptions.style.display = "none";
    const notSupported = document.querySelector("#warn-not-supported");
    notSupported.style.display = "block";
}
function renderSubs() {
    return __awaiter(this, void 0, void 0, function* () {
        const subscription = yield pushCenter.getSubscription();
        const typesPromise = subscriptionService.getSubscriptionTypes();
        let activeSubs = new Array();
        if (subscription) {
            const activeSubsPromise = subscriptionService.getActiveSubscription(subscription.endpoint);
            activeSubs = yield activeSubsPromise;
        }
        const subscriptions = document.querySelector("#subscriptions");
        while (subscriptions.firstChild) {
            subscriptions.removeChild(subscriptions.firstChild);
        }
        const types = yield typesPromise;
        for (let typeId in types) {
            const t = document.querySelector('#tmpl-subscription');
            const tmpl = document.importNode(t.content, true);
            const nameSpan = tmpl.querySelector(".subscription-name");
            nameSpan.textContent = types[typeId];
            const isSubbed = activeSubs.find(s => s.toString() == typeId) != undefined;
            const subBtn = tmpl.querySelector(".subscribe-btn");
            subBtn.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                yield pushCenter.subscribe(parseInt(typeId));
                renderSubs();
            }), true);
            subBtn.disabled = isSubbed;
            const unsubBtn = tmpl.querySelector(".unsubscribe-btn");
            unsubBtn.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                yield pushCenter.unsubscribe(parseInt(typeId));
                renderSubs();
            }));
            unsubBtn.disabled = !isSubbed;
            const testBtn = tmpl.querySelector(".test-btn");
            testBtn.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                yield fetch(`api/send/toSubscriber?type=${typeId}`, {
                    method: 'post',
                    headers: { "Content-type": "application/json" },
                    body: JSON.stringify({ endpoint: subscription.endpoint, sendNotificationModel: { body: "My Message", title: "My Title" } })
                });
            }));
            testBtn.disabled = !isSubbed;
            subscriptions.appendChild(tmpl);
        }
        const loading = document.querySelector("#loading");
        loading.style.display = "none";
        subscriptions.style.display = "block";
    });
}
//# sourceMappingURL=index.js.map