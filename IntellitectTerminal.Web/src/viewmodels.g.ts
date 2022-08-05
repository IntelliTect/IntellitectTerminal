import * as $metadata from './metadata.g'
import * as $models from './models.g'
import * as $apiClients from './api-clients.g'
import { ViewModel, ListViewModel, ServiceViewModel, DeepPartial, defineProps } from 'coalesce-vue/lib/viewmodel'

export interface ChallengeViewModel extends $models.Challenge {
  challengeId: number | null;
  question: string | null;
  answer: string | null;
  level: number | null;
  compilationLanguage: $models.CompilationLanguages | null;
}
export class ChallengeViewModel extends ViewModel<$models.Challenge, $apiClients.ChallengeApiClient, number> implements $models.Challenge  {
  
  constructor(initialData?: DeepPartial<$models.Challenge> | null) {
    super($metadata.Challenge, new $apiClients.ChallengeApiClient(), initialData)
  }
}
defineProps(ChallengeViewModel, $metadata.Challenge)

export class ChallengeListViewModel extends ListViewModel<$models.Challenge, $apiClients.ChallengeApiClient, ChallengeViewModel> {
  
  constructor() {
    super($metadata.Challenge, new $apiClients.ChallengeApiClient())
  }
}


export interface SubmissionViewModel extends $models.Submission {
  submissionId: number | null;
  user: UserViewModel | null;
  challenge: ChallengeViewModel | null;
  content: string | null;
}
export class SubmissionViewModel extends ViewModel<$models.Submission, $apiClients.SubmissionApiClient, number> implements $models.Submission  {
  
  constructor(initialData?: DeepPartial<$models.Submission> | null) {
    super($metadata.Submission, new $apiClients.SubmissionApiClient(), initialData)
  }
}
defineProps(SubmissionViewModel, $metadata.Submission)

export class SubmissionListViewModel extends ListViewModel<$models.Submission, $apiClients.SubmissionApiClient, SubmissionViewModel> {
  
  constructor() {
    super($metadata.Submission, new $apiClients.SubmissionApiClient())
  }
}


export interface UserViewModel extends $models.User {
  userId: string | null;
  fileSystem: string | null;
}
export class UserViewModel extends ViewModel<$models.User, $apiClients.UserApiClient, string> implements $models.User  {
  
  constructor(initialData?: DeepPartial<$models.User> | null) {
    super($metadata.User, new $apiClients.UserApiClient(), initialData)
  }
}
defineProps(UserViewModel, $metadata.User)

export class UserListViewModel extends ListViewModel<$models.User, $apiClients.UserApiClient, UserViewModel> {
  
  constructor() {
    super($metadata.User, new $apiClients.UserApiClient())
  }
}


export class CommandServiceViewModel extends ServiceViewModel<typeof $metadata.CommandService, $apiClients.CommandServiceApiClient> {
  
  public get requestCommand() {
    const requestCommand = this.$apiClient.$makeCaller(
      this.$metadata.methods.requestCommand,
      (c, userId: string | null) => c.requestCommand(userId),
      () => ({userId: null as string | null, }),
      (c, args) => c.requestCommand(args.userId))
    
    Object.defineProperty(this, 'requestCommand', {value: requestCommand});
    return requestCommand
  }
  
  constructor() {
    super($metadata.CommandService, new $apiClients.CommandServiceApiClient())
  }
}


const viewModelTypeLookup = ViewModel.typeLookup = {
  Challenge: ChallengeViewModel,
  Submission: SubmissionViewModel,
  User: UserViewModel,
}
const listViewModelTypeLookup = ListViewModel.typeLookup = {
  Challenge: ChallengeListViewModel,
  Submission: SubmissionListViewModel,
  User: UserListViewModel,
}
const serviceViewModelTypeLookup = ServiceViewModel.typeLookup = {
  CommandService: CommandServiceViewModel,
}

