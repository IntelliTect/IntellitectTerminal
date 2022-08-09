import * as $metadata from './metadata.g'
import * as $models from './models.g'
import * as $apiClients from './api-clients.g'
import { ViewModel, ListViewModel, ServiceViewModel, DeepPartial, defineProps } from 'coalesce-vue/lib/viewmodel'

export interface ChallengeViewModel extends $models.Challenge {
  challengeId: number | null;
  question: string | null;
  level: number | null;
  compilationLanguage: $models.CompilationLanguages | null;
  answer: string | null;
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
  isCorrect: boolean | null;
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
  creationTime: Date | null;
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
  
  public get request() {
    const request = this.$apiClient.$makeCaller(
      this.$metadata.methods.request,
      (c, userId: string | null) => c.request(userId),
      () => ({userId: null as string | null, }),
      (c, args) => c.request(args.userId))
    
    Object.defineProperty(this, 'request', {value: request});
    return request
  }
  
  public get cat() {
    const cat = this.$apiClient.$makeCaller(
      this.$metadata.methods.cat,
      (c, userId: string | null, fileName: string | null) => c.cat(userId, fileName),
      () => ({userId: null as string | null, fileName: null as string | null, }),
      (c, args) => c.cat(args.userId, args.fileName))
    
    Object.defineProperty(this, 'cat', {value: cat});
    return cat
  }
  
  public get progress() {
    const progress = this.$apiClient.$makeCaller(
      this.$metadata.methods.progress,
      (c, userId: string | null) => c.progress(userId),
      () => ({userId: null as string | null, }),
      (c, args) => c.progress(args.userId))
    
    Object.defineProperty(this, 'progress', {value: progress});
    return progress
  }
  
  public get verify() {
    const verify = this.$apiClient.$makeCaller(
      this.$metadata.methods.verify,
      (c, userId: string | null) => c.verify(userId),
      () => ({userId: null as string | null, }),
      (c, args) => c.verify(args.userId))
    
    Object.defineProperty(this, 'verify', {value: verify});
    return verify
  }
  
  public get submitFile() {
    const submitFile = this.$apiClient.$makeCaller(
      this.$metadata.methods.submitFile,
      (c, file: File | null, userId: string | null) => c.submitFile(file, userId),
      () => ({file: null as File | null, userId: null as string | null, }),
      (c, args) => c.submitFile(args.file, args.userId))
    
    Object.defineProperty(this, 'submitFile', {value: submitFile});
    return submitFile
  }
  
  public get submitUserInput() {
    const submitUserInput = this.$apiClient.$makeCaller(
      this.$metadata.methods.submitUserInput,
      (c, input: string | null, userId: string | null) => c.submitUserInput(input, userId),
      () => ({input: null as string | null, userId: null as string | null, }),
      (c, args) => c.submitUserInput(args.input, args.userId))
    
    Object.defineProperty(this, 'submitUserInput', {value: submitUserInput});
    return submitUserInput
  }
  
  constructor() {
    super($metadata.CommandService, new $apiClients.CommandServiceApiClient())
  }
}


export class UserServiceViewModel extends ServiceViewModel<typeof $metadata.UserService, $apiClients.UserServiceApiClient> {
  
  public get initializeFileSystem() {
    const initializeFileSystem = this.$apiClient.$makeCaller(
      this.$metadata.methods.initializeFileSystem,
      (c, userId: string | null) => c.initializeFileSystem(userId),
      () => ({userId: null as string | null, }),
      (c, args) => c.initializeFileSystem(args.userId))
    
    Object.defineProperty(this, 'initializeFileSystem', {value: initializeFileSystem});
    return initializeFileSystem
  }
  
  constructor() {
    super($metadata.UserService, new $apiClients.UserServiceApiClient())
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
  UserService: UserServiceViewModel,
}

