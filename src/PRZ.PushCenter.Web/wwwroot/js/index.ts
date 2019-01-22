import { PushCenter } from "@js/pushCenter";
import { SubscriptionService } from "@js/subscriptionService";

export interface PushCenterWindow extends Window {
    PushCenter: any;
}

const pushCenter = new PushCenter();
const subscriptionService = new SubscriptionService();

declare var window: PushCenterWindow;
window.PushCenter = pushCenter;

renderSubs();

async function renderSubs() {
    const subscription = await pushCenter.getSubscription();
    const typesPromise = subscriptionService.getSubscriptionTypes();
    let activeSubs = [];

    if (subscription) {
        const activeSubsPromise = subscriptionService.getActiveSubscription(subscription.endpoint);
        activeSubs = await activeSubsPromise;
    }

    const types = await typesPromise;

    const root = document.querySelector("#subscriptions");
    while (root.firstChild) {
        root.removeChild(root.firstChild);
    }

    for (let typeId in types) {
        const t = <HTMLTemplateElement>document.querySelector('#tmpl-subscription');
        const tmpl = document.importNode(t.content, true);

        const nameSpan = tmpl.querySelector(".subscription-name");
        nameSpan.textContent = types[typeId];

        const isSubbed = activeSubs.find(s => s.toString() == typeId) != undefined;

        const subBtn = <HTMLButtonElement>tmpl.querySelector(".subscribe-btn");
        subBtn.addEventListener("click", async () => {
            await pushCenter.subscribe(parseInt(typeId));
            renderSubs();
        }, true);
        subBtn.disabled = isSubbed;

        const unsubBtn = <HTMLButtonElement>tmpl.querySelector(".unsubscribe-btn");
        unsubBtn.addEventListener("click", async () => {
            await pushCenter.unsubscribe(parseInt(typeId));
            renderSubs();
        });
        unsubBtn.disabled = !isSubbed;

        root.appendChild(tmpl)
    }
}