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
  public request(userId: string | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<$models.Challenge>> {
    const $method = this.$metadata.methods.request
    const $params =  {
      userId,
    }
    return this.$invoke($method, $params, $config)
  }
  
}


export class UserServiceApiClient extends ServiceApiClient<typeof $metadata.UserService> {
  constructor() { super($metadata.UserService) }
  public initializeFileSystem(user: $models.User | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<$models.User>> {
    const $method = this.$metadata.methods.initializeFileSystem
    const $params =  {
      user,
    }
    return this.$invoke($method, $params, $config)
  }
  
}


