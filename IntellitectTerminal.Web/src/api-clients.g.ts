import * as $metadata from './metadata.g'
import * as $models from './models.g'
import { AxiosClient, ModelApiClient, ServiceApiClient, ItemResult, ListResult } from 'coalesce-vue/lib/api-client'
import { AxiosPromise, AxiosResponse, AxiosRequestConfig } from 'axios'

export class ChallengeApiClient extends ModelApiClient<$models.Challenge> {
  constructor() { super($metadata.Challenge) }
}


export class SubmissionApiClient extends ModelApiClient<$models.Submission> {
  constructor() { super($metadata.Submission) }
}


export class UserApiClient extends ModelApiClient<$models.User> {
  constructor() { super($metadata.User) }
}


export class CommandServiceApiClient extends ServiceApiClient<typeof $metadata.CommandService> {
  constructor() { super($metadata.CommandService) }
  public request(userId: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<string>> {
    const $method = this.$metadata.methods.request
    const $params =  {
      userId,
    }
    return this.$invoke($method, $params, $config)
  }
  
  public cat(userId: string | null, fileName: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<string>> {
    const $method = this.$metadata.methods.cat
    const $params =  {
      userId,
      fileName,
    }
    return this.$invoke($method, $params, $config)
  }
  
  public progress(userId: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<string>> {
    const $method = this.$metadata.methods.progress
    const $params =  {
      userId,
    }
    return this.$invoke($method, $params, $config)
  }
  
  public verify(userId: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<boolean>> {
    const $method = this.$metadata.methods.verify
    const $params =  {
      userId,
    }
    return this.$invoke($method, $params, $config)
  }
  
  public submitFile(file: File | null, userId: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<void>> {
    const $method = this.$metadata.methods.submitFile
    const $params =  {
      file,
      userId,
    }
    return this.$invoke($method, $params, $config)
  }
  
  public submitUserInput(input: string | null, userId: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<void>> {
    const $method = this.$metadata.methods.submitUserInput
    const $params =  {
      input,
      userId,
    }
    return this.$invoke($method, $params, $config)
  }
  
}


export class UserServiceApiClient extends ServiceApiClient<typeof $metadata.UserService> {
  constructor() { super($metadata.UserService) }
  public initializeFileSystem(userId: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<$models.User>> {
    const $method = this.$metadata.methods.initializeFileSystem
    const $params =  {
      userId,
    }
    return this.$invoke($method, $params, $config)
  }
  
}


