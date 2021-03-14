import { PushCenter } from "pushCenter"
import { SubscriptionService } from "subscriptionService";

export interface PushCenterWindow extends Window {
    PushCenter: any;
}

const pushCenter = new PushCenter();
const subscriptionService = new SubscriptionService();

declare var window: PushCenterWindow;
window.PushCenter = pushCenter;

if (pushCenter.isSupported) {
    renderSubs();
} else {
    renderNotSupported();
}

function renderNotSupported() {
    const loading = document.querySelector("#loading") as HTMLElement;
    loading.style.display = "none";

    const subscriptions = document.querySelector("#subscriptions") as HTMLElement;
    subscriptions.style.display = "none";

    const notSupported = document.querySelector("#warn-not-supported") as HTMLElement;
    notSupported.style.display = "block";
}

async function renderSubs() {
    const subscription = await pushCenter.getSubscription();
    const typesPromise = subscriptionService.getSubscriptionTypes();
    let activeSubs = new Array<number>();

    if (subscription) {
        const activeSubsPromise = subscriptionService.getActiveSubscription(subscription.endpoint);
        activeSubs = await activeSubsPromise;
    }

    const subscriptions = document.querySelector("#subscriptions") as HTMLElement;
    while (subscriptions.firstChild) {
        subscriptions.removeChild(subscriptions.firstChild);
    }

    const types = await typesPromise;
    for (let typeId in types) {
        const t = <HTMLTemplateElement>document.querySelector('#tmpl-subscription');
        const tmpl = document.importNode(t.content, true);

        const nameSpan = tmpl.querySelector(".subscription-name");
        nameSpan!!.textContent = types[typeId];

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

        const testBtn = <HTMLButtonElement>tmpl.querySelector(".test-btn");
        testBtn.addEventListener("click", async () => {
            await fetch(`api/send/toSubscriber?type=${typeId}`, {
                method: 'post',
                headers: { "Content-type": "application/json" },
                body: JSON.stringify({ endpoint: subscription!!.endpoint, sendNotificationModel: { body: "My Message", title: "My Title" } })
            });
        });
        testBtn.disabled = !isSubbed;

        subscriptions.appendChild(tmpl)
    }

    const loading = document.querySelector("#loading") as HTMLElement;
    loading.style.display = "none";
    subscriptions.style.display = "block";
}