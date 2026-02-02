<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('firstName','lastName','email','username','password','password-confirm'); section>

    <#if section = "header">
        <h1 class="text-3xl font-bold text-center text-gray-800">
            Create Account
        </h1>
        <p class="text-gray-500 text-center mt-2">Join us today</p>
    </#if>

    <#if section = "form">
        <div class="mt-8">
            <form action="${url.registrationAction}" method="post" class="space-y-5">
                
                <#-- Name Fields -->
                <div class="grid grid-cols-2 gap-4">
                    <div>
                        <label for="firstName" class="input-label">${msg("firstName")}</label>
                        <input 
                            type="text" 
                            id="firstName" 
                            name="firstName" 
                            class="input-field <#if messagesPerField.existsError('firstName')>border-red-500</#if>"
                            value="${(register.formData.firstName!'')}"
                        />
                    </div>
                    <div>
                        <label for="lastName" class="input-label">${msg("lastName")}</label>
                        <input 
                            type="text" 
                            id="lastName" 
                            name="lastName" 
                            class="input-field <#if messagesPerField.existsError('lastName')>border-red-500</#if>"
                            value="${(register.formData.lastName!'')}"
                        />
                    </div>
                </div>

                <#-- Email -->
                <div>
                    <label for="email" class="input-label">${msg("email")}</label>
                    <input 
                        type="email" 
                        id="email" 
                        name="email" 
                        class="input-field <#if messagesPerField.existsError('email')>border-red-500</#if>"
                        value="${(register.formData.email!'')}"
                        autocomplete="email"
                    />
                    <#if messagesPerField.existsError('email')>
                        <p class="mt-1 text-sm text-red-600">${kcSanitize(messagesPerField.get('email'))?no_esc}</p>
                    </#if>
                </div>

                <#-- Username (if not using email as username) -->
                <#if !realm.registrationEmailAsUsername>
                    <div>
                        <label for="username" class="input-label">${msg("username")}</label>
                        <input 
                            type="text" 
                            id="username" 
                            name="username" 
                            class="input-field <#if messagesPerField.existsError('username')>border-red-500</#if>"
                            value="${(register.formData.username!'')}"
                            autocomplete="username"
                        />
                        <#if messagesPerField.existsError('username')>
                            <p class="mt-1 text-sm text-red-600">${kcSanitize(messagesPerField.get('username'))?no_esc}</p>
                        </#if>
                    </div>
                </#if>

                <#-- Password -->
                <div>
                    <label for="password" class="input-label">${msg("password")}</label>
                    <input 
                        type="password" 
                        id="password" 
                        name="password" 
                        class="input-field <#if messagesPerField.existsError('password')>border-red-500</#if>"
                        autocomplete="new-password"
                    />
                    <#if messagesPerField.existsError('password')>
                        <p class="mt-1 text-sm text-red-600">${kcSanitize(messagesPerField.get('password'))?no_esc}</p>
                    </#if>
                </div>

                <#-- Confirm Password -->
                <div>
                    <label for="password-confirm" class="input-label">${msg("passwordConfirm")}</label>
                    <input 
                        type="password" 
                        id="password-confirm" 
                        name="password-confirm" 
                        class="input-field <#if messagesPerField.existsError('password-confirm')>border-red-500</#if>"
                        autocomplete="new-password"
                    />
                    <#if messagesPerField.existsError('password-confirm')>
                        <p class="mt-1 text-sm text-red-600">${kcSanitize(messagesPerField.get('password-confirm'))?no_esc}</p>
                    </#if>
                </div>

                <#-- Submit -->
                <div>
                    <button type="submit" class="btn-primary">
                        ${msg("doRegister")}
                    </button>
                </div>

                <#-- Back to Login -->
                <div class="text-center">
                    <a href="${url.loginUrl}" class="text-hotel-primary hover:text-hotel-secondary text-sm font-semibold">
                        ← ${msg("backToLogin")}
                    </a>
                </div>
            </form>
        </div>
    </#if>

</@layout.registrationLayout>