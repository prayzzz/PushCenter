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
exports.SubscriptionService = void 0;
class SubscriptionService {
    getSubscriptionTypes() {
        return __awaiter(this, void 0, void 0, function* () {
            const req = yield fetch(`api/subscription/subscription-types`);
            if (!req.ok) {
                return {};
            }
            return req.json();
        });
    }
    getActiveSubscription(endpoint) {
        return __awaiter(this, void 0, void 0, function* () {
            const req = yield fetch(`api/subscription/find`, {
                method: 'post',
                headers: { "Content-type": "text/plain" },
                body: endpoint
            });
            if (!req.ok) {
                return [];
            }
            return req.json();
        });
    }
}
exports.SubscriptionService = SubscriptionService;
//# sourceMappingURL=subscriptionService.js.map