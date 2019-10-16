﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Schema;

namespace SkillHost.Bots
{
    public class SkillHostBot : ActivityHandler
    {
        private readonly IStatePropertyAccessor<string> _activeSkillProperty;
        private readonly ConversationState _conversationState;

        public SkillHostBot(ConversationState conversationState)
        {
            _conversationState = conversationState;
            _activeSkillProperty = conversationState.CreateProperty<string>("activeSkillProperty");
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // if there is an active skill
            var activeSkillId = await _activeSkillProperty.GetAsync(turnContext, () => null, cancellationToken);
            if (activeSkillId != null)
            {
                // NOTE: Always SaveChanges() before calling a skill so that any activity generated by the skill will have access to current accurate state.
                await _conversationState.SaveChangesAsync(turnContext, force: true, cancellationToken: cancellationToken);

                // route activity to the skill
                await turnContext.TurnState.Get<SkillAdapter>().ForwardActivityAsync(turnContext, activeSkillId, (Activity)turnContext.Activity, cancellationToken);
            }
            else
            {
                if (turnContext.Activity.Text.Contains("skill"))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Got it, connecting you to the skill..."), cancellationToken);

                    // save conversationReference for skill
                    await _activeSkillProperty.SetAsync(turnContext, "EchoSkill", cancellationToken);

                    // NOTE: Always SaveChanges() before calling a skill so that any activity generated by the skill will have access to current accurate state.
                    await _conversationState.SaveChangesAsync(turnContext, force: true, cancellationToken: cancellationToken);

                    // route the activity to the skill
                    await turnContext.TurnState.Get<SkillAdapter>().ForwardActivityAsync(turnContext, "EchoSkill", (Activity)turnContext.Activity, cancellationToken);
                }
                else
                {
                    // just respond
                    await turnContext.SendActivityAsync(MessageFactory.Text("Me no nothin'. Say \"skill\" and I'll patch you through"), cancellationToken);
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Hello and welcome!"), cancellationToken);
                }
            }
        }

        protected override async Task OnUnrecognizedActivityTypeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity;
            if (turnContext.Activity.Type == ActivityTypes.EndOfConversation)
            {
                // TODO probably don't need skillId anymore
                if (activity.Recipient.Properties.ContainsKey("skillId"))
                {
                    var skillId = (string)activity.Recipient.Properties["skillId"];
                    var activeSkillId = await _activeSkillProperty.GetAsync(turnContext, () => null, cancellationToken);
                    if (activeSkillId == skillId)
                    {
                        // forget skill invocation
                        await _activeSkillProperty.DeleteAsync(turnContext, cancellationToken);
                        await _conversationState.SaveChangesAsync(turnContext, force: true, cancellationToken: cancellationToken);
                    }

                    // We are back
                    await turnContext.SendActivityAsync(MessageFactory.Text("Back in the root bot. Say \"skill\" and I'll patch you through"), cancellationToken);

                    return;
                }
            }

            await base.OnUnrecognizedActivityTypeAsync(turnContext, cancellationToken);
        }
    }
}
