<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('username','password') displayInfo=realm.password && realm.registrationAllowed && !registrationDisabled??; section>
    
    <#if section = "header">
        <h1 class="text-3xl font-bold text-center text-gray-800">
            Welcome Back
        </h1>
        <p class="text-gray-500 text-center mt-2">Sign in to your account</p>
    </#if>

    <#if section = "form">
        <div class="mt-8">
            <form action="${url.loginAction}" method="post" class="space-y-6">
                
                <#-- Username/Email Field -->
                <div>
                    <label for="username" class="input-label">
                        <#if !realm.loginWithEmailAllowed>${msg("username")}<#elseif !realm.registrationEmailAsUsername>${msg("usernameOrEmail")}<#else>${msg("email")}</#if>
                    </label>
                    <input 
                        type="text" 
                        id="username" 
                        name="username" 
                        class="input-field <#if messagesPerField.existsError('username','password')>border-red-500</#if>"
                        value="${(login.username!'')}"
                        autocomplete="username"
                        autofocus
                    />
                    <#if messagesPerField.existsError('username','password')>
                        <p class="mt-2 text-sm text-red-600">
                            ${kcSanitize(messagesPerField.getFirstError('username','password'))?no_esc}
                        </p>
                    </#if>
                </div>

                <#-- Password Field -->
                <div>
                    <div class="flex justify-between items-center">
                        <label for="password" class="input-label">
                            ${msg("password")}
                        </label>
                        <#if realm.resetPasswordAllowed>
                            <a href="${url.loginResetCredentialsUrl}" class="text-sm text-hotel-primary hover:text-hotel-secondary">
                                ${msg("doForgotPassword")}
                            </a>
                        </#if>
                    </div>
                    <input 
                        type="password" 
                        id="password" 
                        name="password" 
                        class="input-field"
                        autocomplete="current-password"
                    />
                </div>

                <#-- Remember Me -->
                <#if realm.rememberMe && !usernameEditDisabled??>
                    <div class="flex items-center">
                        <input 
                            type="checkbox" 
                            id="rememberMe" 
                            name="rememberMe"
                            class="h-4 w-4 text-hotel-primary focus:ring-hotel-primary border-gray-300 rounded"
                            <#if login.rememberMe??>checked</#if>
                        />
                        <label for="rememberMe" class="ml-2 block text-sm text-gray-700">
                            ${msg("rememberMe")}
                        </label>
                    </div>
                </#if>

                <#-- Submit Button -->
                <div>
                    <button type="submit" class="btn-primary">
                        ${msg("doLogIn")}
                    </button>
                </div>
            </form>
        </div>
    </#if>

    <#if section = "info">
        <#if realm.password && realm.registrationAllowed && !registrationDisabled??>
            <div class="mt-6 text-center">
                <p class="text-gray-600">
                    ${msg("noAccount")} 
                    <a href="${url.registrationUrl}" class="text-hotel-primary hover:text-hotel-secondary font-semibold">
                        ${msg("doRegister")}
                    </a>
                </p>
            </div>
        </#if>
    </#if>

</@layout.registrationLayout>