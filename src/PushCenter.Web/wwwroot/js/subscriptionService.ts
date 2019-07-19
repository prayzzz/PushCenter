import { Dictionary } from "@js/Dictionary";

export class SubscriptionService {
    public async getSubscriptionTypes(): Promise<Dictionary<string>> {
        const req = await fetch(`api/subscription/subscription-types`);
        if (!req.ok) {
            return {};
        }

        return req.json()
    }

    public async getActiveSubscription(endpoint: string): Promise<Array<number>> {
        const req = await fetch(`api/subscription/find`, {
            method: 'post',
            headers: { "Content-type": "text/plain" },
            body: endpoint
        });
        if (!req.ok) {
            return [];
        }

        return req.json()
    }
}